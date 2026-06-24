$ErrorActionPreference = 'Stop'

$acr     = $env:_PS_ACR
$display = $env:_PS_DISPLAY
$author  = $env:_PS_AUTHOR
$ver     = $env:_PS_VER
$job     = $env:_PS_JOB
$jobId   = $env:_PS_JOBID
$dir     = $env:_PS_DIR.TrimEnd('\')

if (-not $acr -or -not $dir -or -not (Test-Path $dir)) {
    Write-Host "[ERROR] Env vars not set or Init dir not found!"
    exit 1
}

function ReadUtf8($path) {
    return [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)
}
function WriteUtf8($path, $content) {
    [System.IO.File]::WriteAllText($path, $content, [System.Text.Encoding]::UTF8)
}

# ================================================================
#  IMPORTANT: targeted replacements MUST run BEFORE blanket Init->Acr
# ================================================================

# ---- 1. csproj output path: ACR\Init -> ACR\Author -------------
Write-Host "[STEP 1/8] csproj output path (ACR\Init -> ACR\$author)"
$csproj = Get-ChildItem -Path $dir -Filter '*.csproj' | Select-Object -First 1
if ($csproj) {
    $txt = ReadUtf8 $csproj.FullName
    $txt = $txt -replace 'ACR\\Init', ('ACR\' + $author)
    WriteUtf8 $csproj.FullName $txt
    Write-Host "  OK -> ACR\$author"
}

# ---- 2. RotationMetadata: job + display/author/version ----------
Write-Host "[STEP 2/8] RotationMetadata (Job.MCH -> Job.$job, meta fix)"
$rot = Get-ChildItem -Path $dir -Filter '*Rotation.cs' -Recurse | Select-Object -First 1
if ($rot) {
    $txt = ReadUtf8 $rot.FullName
    $txt = $txt -replace 'Job\.\w+', ('Job.' + $job)
    $oldMeta = '"Init", "Init", "1.0.0.0"'
    $newMeta = '"' + $display + '", "' + $author + '", "' + $ver + '"'
    $txt = $txt -replace [regex]::Escape($oldMeta), $newMeta
    WriteUtf8 $rot.FullName $txt
    Write-Host "  OK -> Job.$job  $newMeta"
}

# ---- 3. macro command: /Init -> /Acr ---------------------------
Write-Host "[STEP 3/8] Chat command (/Init -> /$acr)"
$macro = Get-ChildItem -Path $dir -Recurse -Filter '*MacroManager.cs' | Select-Object -First 1
if ($macro) {
    $txt = ReadUtf8 $macro.FullName
    $txt = $txt -replace '/Init\b', ('/' + $acr)
    WriteUtf8 $macro.FullName $txt
    Write-Host "  OK -> /$acr"
}

# ---- 4. JobHelper -> {Job}Helper (file rename + content) --------
Write-Host "[STEP 4/8] JobHelper -> ${job}Helper"
$jh = Get-ChildItem -Path $dir -Recurse -File | Where-Object { $_.Name -eq 'JobHelper.cs' } | Select-Object -First 1
if ($jh) {
    $jhNewName = $job + 'Helper.cs'
    Rename-Item -Path $jh.FullName -NewName $jhNewName -Force
    Write-Host "  File: JobHelper.cs -> $jhNewName"
}
# Content replacement in all files
$allBefore = @(Get-ChildItem -Path $dir -Recurse -Include *.cs,*.csproj,*.md |
    Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\' })
foreach ($f in $allBefore) {
    $t = ReadUtf8 $f.FullName
    if ($t -match '\bJobHelper\b') {
        $t = $t -replace '\bJobHelper\b', ($job + 'Helper')
        WriteUtf8 $f.FullName $t
    }
}
Write-Host "  Content: JobHelper -> ${job}Helper updated"

# ---- 5. BLANKET: Init -> Acr in all file contents ---------------
Write-Host "[STEP 5/8] Init -> $acr in all file contents"
$files = @(Get-ChildItem -Path $dir -Recurse -Include *.cs,*.csproj,*.md |
    Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\' })
$parentDir = Split-Path $dir -Parent
$count = 0
foreach ($f in $files) {
    $text = ReadUtf8 $f.FullName
    $orig = $text
    $text = $text -replace '\bInit(?=[A-Z])', $acr   # InitRotation -> BardRotation
    $text = $text -replace '\bInit\b', $acr           # namespace Init;  [Init]
    $text = $text -replace '"Init"', ('"' + $author + '"')
    $text = $text -replace 'Init\.Settings\.json', ($acr + '.Settings.json')
    if ($text -ne $orig) { WriteUtf8 $f.FullName $text; $count++ }
}
Write-Host "  Modified $count files"

# ---- 6. rename Init*.cs files -> Acr*.cs ------------------------
Write-Host "[STEP 6/8] Rename Init*.cs -> ${acr}*.cs"
$ren = 0
Get-ChildItem -Path $dir -Recurse -File | Where-Object { $_.Name -match '^Init' } |
    ForEach-Object {
        $nn = $_.Name -replace '^Init', $acr
        Write-Host "  $($_.Name) -> $nn"
        Rename-Item -Path $_.FullName -NewName $nn -Force
        $ren++
    }
Write-Host "  Renamed $ren files"

# ---- 7. rename skill dir ----------------------------------------
Write-Host "[STEP 7/8] Rename skill dir (init-dev -> ${acr}-dev)"
$sd = Join-Path $dir '.claude\skills\init-dev'
if (Test-Path $sd) {
    Rename-Item -Path $sd -NewName ($acr.ToLower() + '-dev') -Force
    Write-Host "  OK -> $($acr.ToLower())-dev"
}

# ---- 8. rename project dir Init/ -> AuthorAcr/ ------------------
Write-Host "[STEP 8/8] Rename project dir (Init -> ${author}${acr})"
$newDir = Join-Path (Split-Path $dir -Parent) ($author + $acr)
if (Test-Path $dir) {
    Rename-Item -Path $dir -NewName ($author + $acr) -Force
    $dir = $newDir
    Write-Host "  OK -> $($author + $acr)"
}

# ---- done -------------------------------------------------------
Write-Host ''
Write-Host '============================================'
Write-Host '  Setup complete!'
Write-Host '============================================'
Write-Host "  Dir:     $dir"
Write-Host "  ACR:     $acr"
Write-Host "  Display: $display"
Write-Host "  Author:  $author"
Write-Host "  Version: $ver"
Write-Host "  Job:     $job (Job.$job)"
Write-Host "  Command: /$acr"
Write-Host ''
Write-Host "  Next: cd $dir && dotnet build"
