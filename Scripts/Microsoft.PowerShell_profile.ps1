$RepoRoot = "D:\Projects\CineMeoTic_BackEnd"

function Get-ApiProjectPath([string]$api_name) {
  $key = $api_name.ToLower()

  $projects = Get-ChildItem -Path $RepoRoot -Recurse -Filter "*.API.csproj" |
    Select-Object -ExpandProperty FullName

  $matches = $projects | Where-Object {
    ([System.IO.Path]::GetFileNameWithoutExtension($_)).ToLower().Contains($key)
  }

  if ($matches.Count -eq 0) {
    throw "Không tìm thấy API '$api_name'. Projects: $($projects | ForEach-Object { [System.IO.Path]::GetFileNameWithoutExtension($_) } | Sort-Object -Unique -join ', ')"
  }

  if ($matches.Count -gt 1) {
    throw "api_name '$api_name' bị trùng. Matches: $($matches -join '; ')"
  }

  # Trả về thư mục chứa .csproj thay vì chính file .csproj
  return [System.IO.Path]::GetDirectoryName($matches[0])
}

function add-migration([string]$api_name, [string]$migration_name, [string]$context = "") {
  $project = Get-ApiProjectPath $api_name
  if ([string]::IsNullOrWhiteSpace($context)) {
    dotnet ef migrations add $migration_name -p $project -s $project
  } else {
    dotnet ef migrations add $migration_name -p $project -s $project -c $context
  }
}

function update-database([string]$api_name, [string]$context = "") {
  $project = Get-ApiProjectPath $api_name
  if ([string]::IsNullOrWhiteSpace($context)) {
    dotnet ef database update -p $project -s $project
  } else {
    dotnet ef database update -p $project -s $project -c $context
  }
}