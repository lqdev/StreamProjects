open System.IO


// let filePath = Path.Combine(__SOURCE_DIRECTORY__,"mybinary.txt")
// printfn "%s" filePath
// let binaryFileBytes = File.ReadAllBytes(filePath)


let getFiles directory = 
    Directory.GetFiles(directory)
    |> Array.filter(fun filePath -> 
        let ext = Path.GetExtension(filePath)
        (ext = ".pdf") ||  (ext = ".txt"))

__SOURCE_DIRECTORY__

let mainFiles = getFiles __SOURCE_DIRECTORY__

mainFiles |> Array.iter(printfn "%s")