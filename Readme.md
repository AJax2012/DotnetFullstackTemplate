# Dotnet Fullstack Template with T-SQL

This template project builds a new project with a Users feature. This template connects to SQL Server express by default, but that can be easily changed.

## Installation

### Required For Installation

You will need to install git and a Dotnet SDK at least 3.1 or later.

### Instructions

1. Open PowerShell/Terminal and navigate to the directory you would like to install the template project  
2. Type the following into your terminal
  - `git clone https://github.com/AJax2012/DotnetFullstackTemplate.git`
  - `dotnet new -i {InstallationDirectory}\.template.config`
3. You will need to close out of your termainal to use the new template
4. Open a new PowerShell window after closing all open terminals
5. Type the following into your terminal: `dotnet new fullstacktemplate-tsql -n {NameOfYourProject} -o {OutputDirectory}-DbName {NameOfTheDatabase}`
