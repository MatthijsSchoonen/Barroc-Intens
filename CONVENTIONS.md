# Conventions

## Introduction
All of this projects's conventions are listed in this file. It should provide a guideline for writing readable and consistent code that can be easily understood by others.

## Conventions

### Use tabs for indentation
Please configure your editor to use tabs instead of spaces. Tabs let everthone use the intentation level that they prefer, instead of having a lot of different indentation levels troughout the project.

### Use PascalCase for..
- Classes
- SingleTons
- members
- functions / methods


### Use camelCase for..
- Variables

### Spaces before `(` everytime, exept for function calls
This convention makes function calls more explicit.

### No so called 'hanging' arguments
When a function has a somewhat long name and numerous arguments. Place each argument on a newline.

Reasons why to avoid hanging arguments:
- Almost only works when spaces are used OR when everyone has the same tabstops (we use tabstops so they can be configured to personal preference).

- When there are more long function like this, it looks
quite rediculous. The functions names and arguments are difficult to read, since they're placed at different spots.

### Functions should be discrete and meaningful actions
Name your functions as a verb that describes what they do. They should do one thing. If that thing needs other things to be done, then they should call other, less abstract helper functions.

This keeps functions small, useful, and easy to read.

### Use English
All variables and function names, comments and documentation should be written in English. This so everyone can understand the project more easily and
everything is more concistent, since it is a convention requirement for most other projects.so everyone can understand the project

### Branches
We work with branches to keep the codebase organized. There are several conventions about this as well. There are different
branches having each it's own purpose.

**Main Branch**
The branch that is present by default in the repository. 
This represents the main version of the application. Whenever working functionality is available, it will be pushed to the main branch.
Therefore, it is not intended for non-working or faulty code to be placed here. In principle, it should also be ready for delivery during development. 
Test the application before pushing to the main branch.

**Development Branch**
A separate branch for the development phase of the application. Features are less tested and may not work perfectly. 
When an issue is completed, it will be merged into the development branch.

**Issues**
For each user story, a separate branch will be created that is derived from the main branch, so everyone has a initial working version.
This way, everyone has an isolated environment to work on, without team members being affected by, for example, non-working functionalities that make the work more difficult.

