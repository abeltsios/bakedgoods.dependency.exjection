rm -rf ./publish/*.nupkg
dotnet clean
dotnet pack -c Release -o ./publish --include-source
ls ./publish/*.nupkg | xargs -I{} dotnet nuget push {} -k=$1 -s https://api.nuget.org/v3/index.json --skip-duplicate


