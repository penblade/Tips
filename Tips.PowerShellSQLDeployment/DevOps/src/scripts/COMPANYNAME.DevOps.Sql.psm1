function Invoke-DeploySql {
    <#
    .SYNOPSIS 
        Deploy SQL files to a database.
    .DESCRIPTION 
        Deploy SQL files to a database.
        User must have permission to perform the steps within the sql file.
        Requires PowerShell module SqlServer installed on the server running the PowerShell script.
    .PARAMETER ServerInstance
        The SQL server instance where the SQL will be deployed.
    .PARAMETER Database
        The database where the SQL will be deployed.
    .PARAMETER SqlFiles
        The SQL files that will be deployed.
    .EXAMPLE
        # Adjust directories to reflect where the module and sql files are located.
        Import-Module ".\COMPANYNAME.DevOps.Sql.psm1" -Force
        $sqlFolder = "..\sql\DATABASE1\"
        $sqlFiles = Get-ChildItem $sqlFolder -Filter *.sql | Sort-Object
        Invoke-DeploySql -ServerInstance SERVERINSTANCE1 -Database DATABASE1 -SqlFiles $sqlFiles
        Import-Module ".\COMPANYNAME.DevOps.Sql.psm1" -Force
        $sqlFolder = "..\sql\DATABASE2\"
        $sqlFiles = Get-ChildItem $sqlFolder -Filter *.sql | Sort-Object
        Invoke-DeploySql -ServerInstance SERVERINSTANCE2 -Database DATABASE2 -SqlFiles $sqlFiles
    #>
    [Cmdletbinding()]
    param(
        [Parameter(Mandatory=$true)]
        [String]$ServerInstance,
        [Parameter(Mandatory=$true)]
        [String]$Database,        
        [Parameter(Mandatory=$true)]
        [Array]$SqlFiles
    )
    Write-Output "$($MyInvocation.MyCommand) - Start"
    $totalSqlFiles = ($SqlFiles | Measure-Object).Count
    Write-Output "Total sql files: $totalSqlFiles"
    $SqlFiles | Foreach-Object {
        $sqlFile = $_.FullName
        Write-Output "Processing file: $sqlFile"
        Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $Database -InputFile $sqlFile
        Write-Output "Completed file:  $sqlFile"
    }
    Write-Output "$($MyInvocation.MyCommand) - End"
}
