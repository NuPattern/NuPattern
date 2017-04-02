# Core Development/Developers Processes
These are notes that core developers of the codebase will need to get started. 
## Where is the core development being done?
At this time, for the purposes of better planning and tracking of planned work, maintaining a backlog, and managing sprints, improvements to the project are being planned, and tracked using the TFS Online Servi(www.tfspreview.com) at the site http://vspat.tfspreview.com, rather than directly on this project site.

**Note**: The 'IssueTracker' and 'Discussions' list on this site are still being used as the public interface to those who are following the project, and for general information, announcements, for capturing feedback, suggestions and tracking issues.
As the planned sprints are completed, the code updates will be posted on this site too in the 'Source Control' tab. 
When releases are cut, those are posted to this site in the 'Downloads' tab, and also on the Visual Studio Gallery. 
## Getting access to TFSPreview
If you wish to gain access to the current 'Product Backlog' and 'Sprint Backlogs', please contact the project owner for permissions, we would be happy to get you involved in the planned work.
At some point in the future, we are hoping that there will be better integration between tfspreview (TFS Service) and CodePlex so that our followers can get a better view of the progress on the project, and we can manage it more transparently from the site. 
## Developing the code
The core committers are developing the project in sprints using the backlog management tools provided by TFS from the TFSPREVIEW service at https://vspat.tfspreview.com.

Code is being developed in forks off the main code branch on this site. 

A fork for a developer is named with their codeplex username and the name of their choosing, and they get a ‘master’ branch automatically to work from.

For example, If i created a fork for the next sprint, I choose the name ‘Sprint 20’, I get a fork at the location: http://vspat.codeplex.com/SourceControl/network/forks/jezzsa/Sprint20.

When the fork is created, the developer gets a ‘master’ branch created for them automatically. They can then create additional ‘topic’ branches, to do their work. How they arrange the branches is up to them. One suggestion is to create a new ‘topic’ branch for each new feature, or bug, and then merge them back into the ‘master’ branch at the end of a story or sprint, or at their own will.

At the end of the sprint, the developers submit a ‘Pull Request’ on their fork. 

The project lead then merges the code from the ‘Pull Request’ into the main branch on this site.

## Code Development Process
### Setting up SCC tools for Visual Studio development
* Install 'GIT for Windows' 
* Install the 'Git Source Control Provider' extension from VSGallery. 
* In VS, configure GIT as the current source control provider. 
### Setting up your TFSPREVIEW account
* Obtain access to the TFSPREVIEW site from the project owner of this site. 
* Login to https://vspat.tfspreview.com. 
### Create you own development fork
* Go to: http://vspat.codeplex.com/SourceControl, and click the ‘Fork’ button
* Name your fork. The suggested convention would be to name it with the name of the sprint (i.e. ‘Sprint 20’).
### To create a local repo at start of the sprint:
* Open Command prompt (as administrator)
* On your local computer, go to C:\Projects 
* Do a 'git-tf clone https://git01.codeplex.com/forks/yourname/forkname  $/yourname/forkname' to create a local repo.
### Develop code in a sprint
* Use Visual Studio, and change your source control provider to the 'Git Source Control Provider'. 
* Browse and update the status of your workitems, add, remove, change them to keep them up to date.
* You may choose to create separate topic branches for each of the stories you work on.
* Develop your code, and regularly commit, your code to your local GIT repo using 'git commit <comment>'.
* Optionally, merge your topic branches to your ‘master’ branch, by switching to your master branch ('git checkout master') and 'git merge topic'.
* On task/story completion (or more regularly), push your changes to your fork at the codeplex site ‘origin’, with 'git push origin'
* Update the status of your workitems.
* On sprint complete, at the codeplex site, select your fork, and submit a ‘Pull Request’.