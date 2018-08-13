# Changelog

All notable changes to **Nuke.GitHub** are documented here.

## v1.3.1:
- Invoke `AssertValid()` on settings before using them to fail early in case of invalid settings

## v1.3.0:
- Add CreatePullRequest, GetReleases and GetRepository tasks
- Make `PublishRelease` invocation consistent with other NUKE tools

## v1.2.1:
- Add option to mark releases as `Prerelease`

## v1.2.0:
- Update to NUKE 0.6.0

## v1.1.0:
- Update to NUKE 0.4.0

## v1.0.2
- Use the official NUKE logo for the package

## v1.0.1
- Refactor code to follow NUKE tooling recommendations
- Provide `Nuke.GitHub.ChangeLogExtensions`. This will likely be merged with NUKE itself
  or be distributed in a separate package in the future

## v1.0.0
- Initial Release
- Supports `PublishRelease`

