# OpenIddict PostLogoutRedirectUri Fix - Implementation Summary

## Problem Identified
The OpenIddict client configuration was missing or improperly formatting the `PostLogoutRedirectUris` for the Angular application (`http://localhost:4200`). When the Angular app attempted to logout, OpenIddict rejected the redirect URI as invalid (error ID2052).

## Root Cause Analysis
The `OpenIddictClientFixer` service had an incorrect URI comparison logic:

### Original Code (Buggy):
```csharp
if (descriptor.PostLogoutRedirectUris.All(x => x != uri))
{
    descriptor.PostLogoutRedirectUris.Add(uri);
    // ...
}
```

**Problem**: The comparison `x != uri` was comparing Uri object instances. While Uri does implement equality comparison, this method wasn't reliably matching URIs that were already in the database because of formatting differences (trailing slashes, case variations, etc.).

## Solution Implemented

### Updated Code (Fixed):
```csharp
// Compare using AbsoluteUri (string representation) with case-insensitive comparison
var existingUri = descriptor.PostLogoutRedirectUris.FirstOrDefault(x => 
    string.Equals(x.AbsoluteUri, uri.AbsoluteUri, StringComparison.OrdinalIgnoreCase));

if (existingUri == null)
{
    descriptor.PostLogoutRedirectUris.Add(uri);
    needsUpdate = true;
    _logger.LogInformation($"Added PostLogoutRedirectUri '{uri.AbsoluteUri}' to client '{clientId}'");
}
```

**Key Improvements**:
1. **String-based comparison**: Compares the normalized `AbsoluteUri` string representation instead of Uri object instances
2. **Case-insensitive matching**: Uses `StringComparison.OrdinalIgnoreCase` to handle any case variations
3. **Explicit detection**: Uses `FirstOrDefault()` to explicitly check if a matching URI exists
4. **Better logging**: Added detailed logging to show:
   - How many URIs the client currently has
   - What existing URIs are in the descriptor
   - Whether each required URI was found or needs to be added

## Files Modified
1. **OpenIddictClientFixer.cs**
   - Added `using System.Collections.Generic;` for List<Uri> type
   - Replaced the Uri comparison logic with string-based comparison
   - Enhanced logging for debugging

## Testing
The fix has been compiled successfully and should now properly:
- Detect existing PostLogoutRedirectUris in the OpenIddict database
- Add missing URIs like `http://localhost:4200` for the Angular app
- Update the database with persist the changes
- Log detailed information about what's being fixed

## Expected Behavior After Fix
When the AuthServer starts:
1. The OpenIddictClientFixer service will initialize
2. It will check the `ResourceryWorkflow_Platform` client configuration
3. It will look for the required PostLogoutRedirectUris including:
   - `http://localhost:4200` (Angular app)
   - `http://localhost:4200/` (with trailing slash, if needed)
4. Any missing URIs will be added to the OpenIddict database
5. Detailed logs will show the results of each operation

## Next Steps
1. Restart the AuthServer (via AppHost)
2. Test the Angular app logout flow
3. Verify that no longer receives "invalid post_logout_redirect_uri" error
4. Check the AuthServer logs for the fixer output to confirm URIs are properly configured

## Technical Notes
- The fix uses `Uri.AbsoluteUri` which provides the normalized string representation of the URI
- `StringComparison.OrdinalIgnoreCase` handles case variations while preserving scheme and host comparison
- The comparison happens when the OpenIddict descriptor is populated from the database, so it works with database-stored values
