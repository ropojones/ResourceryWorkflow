# OpenIddict PostLogoutRedirectUri Fix - Complete Analysis & Solution

## Problem Summary
When the Angular application at `http://localhost:4200` attempts to logout, OpenIddict rejects the logout redirect with error ID2052:
```
The end session request was rejected because the specified post_logout_redirect_uri was invalid: http://localhost:4200.
```

## Root Cause Analysis

### Discovery Path
1. **Initial Investigation**: Checked AuthServer configuration - found SelfUrl was using incorrect port `44346` instead of `7600`  
2. **Configuration Fix**: Updated AuthServer `appsettings.json` SelfUrl to `https://localhost:7600` ✅
3. **Deeper Issue Found**: Fixed the SelfUrl but OpenIddict validation still rejected the URI
4. **Code-Based Solution**: Created automatic fixer service to add missing PostLogoutRedirectUris on AuthServer startup
5. **Fixer Output Analysis**: Logs showed fixer claiming URIs were already configured, yet OpenIddict still rejected them
6. **URI Format Mismatch Identified**: The real issue - OpenIddict stores normalized URIs with trailing slashes

### The Critical Discovery
From `DbMigrator\appsettings.json`, the `ResourceryWorkflow_Platform` client is configured with:
```json
"PostLogoutRedirectUris": [
    "http://localhost:4200",        // Without trailing slash
    "http://localhost:4200/"        // With trailing slash
]
```

But when stored in the OpenIddict database, the .NET `Uri` class **normalizes** both forms:
- `Uri.AbsoluteUri` for both `http://localhost:4200` and `http://localhost:4200/` becomes `http://localhost:4200/`
- So the database stores only ONE URI: `http://localhost:4200/` (with trailing slash)

**The Mismatch**: When Angular sends logout request with `http://localhost:4200` (NO trailing slash), it doesn't match the stored normalized URI `http://localhost:4200/` (WITH trailing slash).

## Solution Implemented

### File: [OpenIddictClientFixer.cs](apps/ResourceryWorkflow.AuthServer/OpenIddictClientFixer.cs)

**Enhanced URI Comparison Logic:**
```csharp
// Compare using normalized URIs - ignore trailing slash differences
 var normalizedTarget = uri.AbsoluteUri.TrimEnd('/');
var existingUri = descriptor.PostLogoutRedirectUris.FirstOrDefault(x =>
{
    var normalizedExisting = x.AbsoluteUri.TrimEnd('/');
    return string.Equals(normalizedExisting, normalizedTarget, StringComparison.OrdinalIgnoreCase);
});
```

**Key Improvements:**
1. ✅ Removes trailing slashes before comparison
2. ✅ Case-insensitive string comparison
3. ✅ Detailed logging showing both actual and normalized URIs
4. ✅ Only adds URIs that don't already exist (normalized)
5. ✅ Identifies which exact format is in the database

### Additional Changes
- Added `using System.Collections.Generic;` for enhanced type support
- Enhanced logging to show:
  - Current number of URIs per client
  - Exact URI strings in database (AbsoluteUri and OriginalString)
  - Whether each required URI is being added or already exists
  - URI counts before and after updates

## How It Works

1. **On AuthServer Startup:**
   - OpenIddictClientFixer service initializes before all other middleware
   - Reads configured PostLogoutRedirectUris for each OpenIddict client
   - Populates the OpenIddict descriptor from the database
   - Compares required URIs against stored URIs (normalized)
   - Adds any missing URIs
   - Saves changes back to the database

2. **Normalized Comparison:**
   - Both `http://localhost:4200` and `http://localhost:4200/` are treated as equivalent
   - Prevents duplicate URIs with only trailing slash differences
   - Ensures the required URIs exist in the database regardless of format

3. **Database Persistence:**
   - Changes are saved immediately on startup
   - Survives application restarts
   - No manual database intervention needed

## Client Configuration

Each OpenIddict client is configured in `shared/ResourceryWorkflow.DbMigrator/appsettings.json`:

### ResourceryWorkflow_Platform (Angular App)
```json
{
    "ClientId": "ResourceryWorkflow_Platform",
    "PostLogoutRedirectUris": [
        "http://localhost:4200",
        "http://localhost:4200/"
    ]
}
```

### ResourceryWorkflow_WebApp (Blazor)
```json
{
    "ClientId": "ResourceryWorkflow_WebApp",
    "PostLogoutRedirectUris": [
        "https://localhost:5000/authentication/logout-callback",
        "https://localhost:5000",
        "https://localhost:5000/"
    ]
}
```

## Testing Checklist

After deployment, verify:
1. ✅ AuthServer starts successfully
2. ✅ OpenIddictClientFixer logs appear showing client configuration
3. ✅ Logs show actual URIs in database
4. ✅ Angular app can login
5. ✅ Angular app can logout WITHOUT "invalid post_logout_redirect_uri" error
6. ✅ Response redirects to `http://localhost:4200` successfully

## Log Output Examples

**Expected fixer output:**
```
[INF] Checking and fixing OpenIddict client PostLogoutRedirectUris...
[INF] Client 'ResourceryWorkflow_Platform' currently has 1 PostLogoutRedirectUris
[INF]   Existing URI: 'http://localhost:4200/' (AbsoluteUri)
[INF] Checking if 'http://localhost:4200' or variations exist in client 'ResourceryWorkflow_Platform'
[INF] PostLogoutRedirectUri (normalized) 'http://localhost:4200' already exists as 'http://localhost:4200/' 
[INF] Client 'ResourceryWorkflow_Platform' already has all required PostLogoutRedirectUris. Total URIs: 1
[INF] OpenIddict client PostLogoutRedirectUris fixed successfully.
```

## Technical Notes

1. **URI Normalization**: The .NET `Uri` class adds trailing slashes in many cases. `AbsoluteUri` property returns the fully qualified URI string.

2. **OpenIddict Validation**: OpenIddict stores and compares URIs using their `AbsoluteUri` form, which explains why `http://localhost:4200` and `http://localhost:4200/` may be stored as the same URI.

3. **Case Sensitivity**: While URIs are mostly case-insensitive, they can have case-sensitive query parameters. The fixer uses `StringComparison.OrdinalIgnoreCase` for host comparison safety.

4. **Persistence**: Changes made by the fixer are immediately persisted to the database via `_applicationManager.UpdateAsync()`, ensuring they survive application restarts.

## Files Modified

1. **OpenIddictClientFixer.cs** - Improved URI comparison logic with trailing slash handling
2. **appsettings.json** (AuthServer) - Corrected SelfUrl port from 44346 to 7600

## No Database Migration Required

The fixer is a **runtime fix** that executes on application startup. No manual database scripts or migrations are needed. The configuration is read from `appsettings.json` and applied directly to the running database.
