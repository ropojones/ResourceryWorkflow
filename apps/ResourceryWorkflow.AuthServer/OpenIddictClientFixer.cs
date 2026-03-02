using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.Uow;

namespace ResourceryWorkflow;

public class OpenIddictClientFixer : ITransientDependency
{
    private readonly IAbpApplicationManager _applicationManager;
    private readonly ILogger<OpenIddictClientFixer> _logger;

    public OpenIddictClientFixer(
        IAbpApplicationManager applicationManager,
        ILogger<OpenIddictClientFixer> logger)
    {
        _applicationManager = applicationManager;
        _logger = logger;
    }

    [UnitOfWork]
    public async Task FixPostLogoutRedirectUrisAsync()
    {
        try
        {
            _logger.LogInformation("=== Starting OpenIddict Client Configuration Fix ===");

            // Fix ResourceryWorkflow_Platform (Angular app)
            // Send both variants to EnsurePostLogoutRedirectUrisAsync; it will normalize to canonical form
            await EnsureRedirectUrisAsync(
                "ResourceryWorkflow_Platform",
                new[] { "http://localhost:4200", "http://localhost:4200/" }
            );
            
            await EnsurePostLogoutRedirectUrisAsync(
                "ResourceryWorkflow_Platform",
                new[] { "http://localhost:4200" }  // Will be normalized to http://localhost:4200/
            );

            // Fix ResourceryWorkflow_WebApp (Blazor)
            await EnsurePostLogoutRedirectUrisAsync(
                "ResourceryWorkflow_WebApp",
                new[] { 
                    "https://localhost:5000/authentication/logout-callback",
                    "https://localhost:5000"  // Will normalizeto https://localhost:5000/
                }
            );

            _logger.LogInformation("=== OpenIddict Client Configuration Fix Completed ===");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CRITICAL ERROR: Failed to fix OpenIddict client configuration. This may cause login/logout failures!");
        }
    }

    private async Task EnsureRedirectUrisAsync(string clientId, string[] redirectUris)
    {
        try
        {
            _logger.LogInformation($"==================== [{clientId}] Redirect URIs START ====================");
            
            var client = await _applicationManager.FindByClientIdAsync(clientId);
            if (client == null)
            {
                _logger.LogWarning($"[{clientId}] Client not found - will be created by DbMigrator");
                return;
            }

            var descriptor = new OpenIddictApplicationDescriptor();
            await _applicationManager.PopulateAsync(descriptor, client);

            _logger.LogWarning($"[{clientId}] Current RedirectUris count: {descriptor.RedirectUris.Count}");
            var index = 1;
            foreach (var uri in descriptor.RedirectUris)
            {
                _logger.LogWarning($"[{clientId}] [{index}] '{uri.OriginalString}'");
                index++;
            }

            // Build desired set - DO NOT normalize, accept as-is for OAuth compatibility
            var desiredUris = new List<Uri>();
            foreach (var uriString in redirectUris)
            {
                if (!string.IsNullOrWhiteSpace(uriString))
                {
                    if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri) && uri.IsWellFormedOriginalString())
                    {
                        desiredUris.Add(uri);
                    }
                    else
                    {
                        _logger.LogError($"[{clientId}] INVALID: '{uriString}'");
                    }
                }
            }

            _logger.LogWarning($"[{clientId}] Desired RedirectUris:");
            foreach (var uri in desiredUris)
            {
                _logger.LogWarning($"[{clientId}]   - '{uri.OriginalString}'");
            }

            // Compare
            var currentOriginals = descriptor.RedirectUris
                .Select(x => x.OriginalString)
                .OrderBy(x => x)
                .ToList();
            
            var desiredOriginals = desiredUris
                .Select(x => x.OriginalString)
                .OrderBy(x => x)
                .ToList();

            bool isSame = currentOriginals.Count == desiredOriginals.Count &&
                          currentOriginals.SequenceEqual(desiredOriginals, StringComparer.Ordinal);

            if (isSame)
            {
                _logger.LogInformation($"[{clientId}] ✓ RedirectUris are correct");
                _logger.LogInformation($"[{clientId}] ==================== [{clientId}] Redirect URIs END ====================");
                return;
            }

            _logger.LogError($"[{clientId}] ✗ RedirectUris mismatch - fixing");
            descriptor.RedirectUris.Clear();

            foreach (var uri in desiredUris)
            {
                descriptor.RedirectUris.Add(uri);
                _logger.LogWarning($"[{clientId}]   + '{uri.OriginalString}'");
            }

            _logger.LogWarning($"[{clientId}] Saving RedirectUris...");
            await _applicationManager.UpdateAsync(client, descriptor);
            _logger.LogInformation($"[{clientId}] ✓ RedirectUris updated");
            _logger.LogInformation($"[{clientId}] ==================== [{clientId}] Redirect URIs END ====================");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[{clientId}] CRITICAL: RedirectUris configuration failed!");
        }
    }

    private async Task EnsurePostLogoutRedirectUrisAsync(string clientId, string[] postLogoutRedirectUris)
    {
        try
        {
            _logger.LogInformation($"==================== [{clientId}] Post-Logout URIs START ====================");
            
            var client = await _applicationManager.FindByClientIdAsync(clientId);
            if (client == null)
            {
                _logger.LogWarning($"[{clientId}] Client not found - will be created by DbMigrator");
                return;
            }

            var descriptor = new OpenIddictApplicationDescriptor();
            await _applicationManager.PopulateAsync(descriptor, client);

            _logger.LogWarning($"[{clientId}] Current PostLogoutRedirectUris count: {descriptor.PostLogoutRedirectUris.Count}");
            var index = 1;
            foreach (var uri in descriptor.PostLogoutRedirectUris)
            {
                _logger.LogWarning($"[{clientId}] [{index}] OriginalString: '{uri.OriginalString}'");
                index++;
            }

            // Build desired set - NORMALIZE ALL to canonical trailing-slash form
            // This prevents database deduplication issues
            var desiredUris = new List<Uri>();
            var seenCanonical = new HashSet<string>(StringComparer.Ordinal);
            
            foreach (var uriString in postLogoutRedirectUris)
            {
                if (!string.IsNullOrWhiteSpace(uriString))
                {
                    // Create canonical form: ensure trailing slash for root URLs
                    string canonicalForm = uriString.EndsWith("/") ? uriString : uriString + "/";
                    
                    if (!seenCanonical.Contains(canonicalForm))
                    {
                        if (Uri.TryCreate(canonicalForm, UriKind.Absolute, out var uri) && uri.IsWellFormedOriginalString())
                        {
                            desiredUris.Add(uri);
                            seenCanonical.Add(canonicalForm);
                            _logger.LogWarning($"[{clientId}] Canonical: '{canonicalForm}' (from '{uriString}')");
                        }
                        else
                        {
                            _logger.LogError($"[{clientId}] INVALID: '{canonicalForm}'");
                        }
                    }
                }
            }

            _logger.LogWarning($"[{clientId}] Need {desiredUris.Count} URIs total (canonical forms):");
            foreach (var uri in desiredUris.OrderBy(x => x.OriginalString))
            {
                _logger.LogWarning($"[{clientId}]   - '{uri.OriginalString}'");
            }

            // Compare
            var currentOriginals = descriptor.PostLogoutRedirectUris
                .Select(x => x.OriginalString)
                .OrderBy(x => x)
                .ToList();
            
            var desiredOriginals = desiredUris
                .Select(x => x.OriginalString)
                .OrderBy(x => x)
                .ToList();

            bool isSame = currentOriginals.Count == desiredOriginals.Count &&
                          currentOriginals.SequenceEqual(desiredOriginals, StringComparer.Ordinal);

            if (isSame)
            {
                _logger.LogInformation($"[{clientId}] ✓ PostLogoutRedirectUris are correct");
                _logger.LogInformation($"[{clientId}] ==================== [{clientId}] Post-Logout URIs END ====================");
                return;
            }

            _logger.LogError($"[{clientId}] ✗ PostLogoutRedirectUris mismatch!");
            _logger.LogError($"[{clientId}] Current: {string.Join(", ", currentOriginals)}");
            _logger.LogError($"[{clientId}] Desired: {string.Join(", ", desiredOriginals)}");
            
            descriptor.PostLogoutRedirectUris.Clear();

            foreach (var uri in desiredUris.OrderBy(x => x.OriginalString))
            {
                descriptor.PostLogoutRedirectUris.Add(uri);
                _logger.LogWarning($"[{clientId}]   + '{uri.OriginalString}'");
            }

            _logger.LogWarning($"[{clientId}] Saving {desiredUris.Count} PostLogoutRedirectUris (canonical)...");
            await _applicationManager.UpdateAsync(client, descriptor);
            _logger.LogWarning($"[{clientId}] ✓ PostLogoutRedirectUris updated");

            // Verify
            var verifyClient = await _applicationManager.FindByClientIdAsync(clientId);
            var verifyDescriptor = new OpenIddictApplicationDescriptor();
            await _applicationManager.PopulateAsync(verifyDescriptor, verifyClient);

            _logger.LogWarning($"[{clientId}] VERIFICATION - After update:");
            index = 1;
            foreach (var uri in verifyDescriptor.PostLogoutRedirectUris.OrderBy(x => x.OriginalString))
            {
                _logger.LogWarning($"[{clientId}] [{index}] '{uri.OriginalString}'");
                index++;
            }

            _logger.LogInformation($"[{clientId}] ==================== [{clientId}] Post-Logout URIs END ====================");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[{clientId}] CRITICAL: PostLogoutRedirectUris configuration failed!");
        }
    }
}
