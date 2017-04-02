# 1.4.25.0
* Support for navigating around the model in Target Path Syntax. (i.e. can navigate to child elements, sibling elements and ancestor elements.
* Support for specifying a 'tag' in Target Path Syntax. (i.e. {"..\~[mytag](mytag)\FolderName)"}
* Support for matching/filtering by 'Tag' in all appropriate library automation classes: Commands, Conditions, ValueProviders that deal with artefact links
* Minor breaking changes to the API for inspecting IProductElement.References
# 1.4.24.0
* Full Support for VS2013. Migrated source code to build in VS2013, and created a version of NuPattern for VS2013
* Moved off using the built in VS MEF container over to using the NuPattern specific one.
* Fixed packaging MSBUILD task that was corrupting VSIX files during the signing process
# 1.3.23.0
Release notes for this release and prior releases is captured in the 'Release Notes.docx' shipped on each release page (as PDF), and can be found in source tree in: '\Docs\ReleaseNotes.docx'