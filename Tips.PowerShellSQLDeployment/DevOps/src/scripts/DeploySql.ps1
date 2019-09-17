<#
.SYNOPSIS 
    Deploy SQL files to a database.  Includes root level exception handling.
.DESCRIPTION 
    Deploy SQL files to a database.  Includes root level exception handling.
    User must have permission to perform the steps within the sql file.
    Requires PowerShell module SqlServer installed on the server running the PowerShell script.
    Intended as a step within an Azure DevOps release definition.
    When run within an Azure DevOps release definition, the agent must have the permissions
    and the server the agent is running on must have the PowerShell module SqlServer installed.
    If you standardize where you install your PowerShell modules following
    PowerShell module recommendations, then consider removing the parameter
    in favor of referencing the module directly.
.PARAMETER DevOpsSqlModule
    The DevOps SQL module file path.
.PARAMETER ServerInstance
    The SQL server instance where the SQL will be deployed.
.PARAMETER Database
    The database where the SQL will be deployed.
.PARAMETER SqlFolder
    The folder containing the .sql files that will be deployed.
.EXAMPLE
    .\DeploySql -DevOpsSqlModule "C:\temp\scripts\COMPANYNAME.DevOps.Sql.psm1" -ServerInstance SERVERINSTANCE1 -Database DATABASE1 -SqlFolder "C:\temp\sql\DATABASE1\"
    .\DeploySql -DevOpsSqlModule "C:\temp\scripts\ COMPANYNAME.DevOps.Sql.psm1" -ServerInstance SERVERINSTANCE2  -Database DATABASE2 -SqlFolder "C:\temp\sql\DATABASE2\"
#>
[Cmdletbinding()]
param(
    [Parameter(Mandatory=$true)]
    [String]$DevOpsSqlModule,
    [Parameter(Mandatory=$true)]
    [String]$ServerInstance,
    [Parameter(Mandatory=$true)]
    [String]$Database,    
    [Parameter(Mandatory=$true)]
    [String]$SqlFolder
)
$scriptFile = Get-Item $PSCommandPath;
Write-Output "$($scriptFile.Name) - Start"
Try {
    Write-Output "Importing DevOps SQL module"
    Import-Module $DevOpsSqlModule -Force
    Write-Output "Applying scripts to server instance '$ServerInstance' on database '$Database'"
    $sqlFiles = Get-ChildItem $SqlFolder -Filter *.sql | Sort-Object
    $totalSqlFilesFound = ($sqlFiles | Measure-Object).Count
    If ($totalSqlFilesFound -eq 0) {
        Write-Output "##vso[task.LogIssue type=warning;]Warning: no sql files found."
    }
    else {
        Invoke-DeploySql -ServerInstance $ServerInstance -Database $Database -SqlFiles $sqlFiles
    }
}
Catch {
    # Azure DevOps errors
    Write-Output "##vso[task.LogIssue type=error;] $($scriptFile.Name)"
    Write-Output "##vso[task.LogIssue type=error;] Script Path: $($scriptFile.FullName)"
    Write-Output "##vso[task.LogIssue type=error;] $_"
    Write-Output "##vso[task.LogIssue type=error;] $($_.ScriptStackTrace)"
    Exit 1
}
Write-Output "$($scriptFile.Name) - End"
