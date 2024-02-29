
- Run migration scripts
  Add-Migration -Name Initial -OutputDir Migrations/Auth -Context CustomerDbContext
  Add-Migration -Name Initial -OutputDir Migrations/Commerce -Context CommerceDbContext