Write-Output "Creating Direcotry in Nop.Data Directory..."

mkdir ..\..\Libraries\Nop.Data\Mapping\PartialPayment

Write-Output "moving mapping files..."

mv .\Mappings\*.cs ..\..\Libraries\Nop.Data\Mapping\PartialPayment\

Remove-Item -Path .\Mappings

Write-Output "Done!"