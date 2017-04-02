# Code Contributor Checklist

This minimal checklist is intended to guide developers and contributors in understanding the minimum bar of code quality that will be accepted in the project. 

The intention behind this checklist is to ensure that all code that is contributed to the project complies with the naming, coding, organizational standards of the project, so that contributions do not generate more unecessary work for all those who are collectively managing and maintaining the lifecycle of the project. There is absolutely no intention to add unecessary beauracracy to the process, and if that is deemed the case, this process and checklist will need to be revised appropriately.

As with any project, there needs to be consistency in how the code is organized, structured and written to make maintenance and understanding of the entire codebase easier for everyone. And it is the responsibility of each contributor to ensure that their code contributions do not decrease that consistency, or increase the maintenace overhead of the project for others.

## The Checklist
Before any code is accepted into the project, all contributors should ensure that they have completed the following checklist of activities that ensure that your code meets the minimum bar of acceptance:
# The code complies with all [Coding Guidelines](Coding-Guidelines). Don't worry, there aren't many, they are all sensible and in wide use, and they are all geared at fitting in with what we already have.
# The code compiles without error, using the Make-All.bat file located in the 'Src' directory. What else would you expect?
# You have at least tried (within reason) to provide any relevant 'Unit', 'Integration' or 'User' tests that verify the code in your new contribution.
# All automated tests ('Unit', 'Integration', 'User') tests and any manual tests pass, in both VS2010 and VS2012 solution configurations. Not much use having them if they prove the code does not do what it is supposed to.
# All guidance documents or release notes are regenerated (if necessary) to reflect any changes you have made to them.
# A 'Pull Request' with your changes is made on the codeplex site.

Failing to meet this minimum quality bar is likely to warrant a reminder from a project contributor to rework your contribution - unless of course you can get someone else to bring your contribution up to the quality bar!