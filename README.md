# Hubtel Take Home

## Database used
   -  Postgres  

- Run migration scripts
  1. Add-Migration -Name Initial -OutputDir Migrations/Auth -Context CustomerDbContext
  2. Add-Migration -Name Initial -OutputDir Migrations/Commerce -Context CommerceDbContext
  3. Update-Database -Context AuthDbContext
  4. Update-Database -Context CommerceDbContext


## Testing Libraries Used
  - Nunit
  - Nsubstitute
  - FluentAssertions

## Other Libraries
  - Serilog
  - Swashbuckle