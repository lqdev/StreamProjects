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

// Get files of specific extension type
let getFiles directory extension = 
    Directory.GetFiles(directory)
    |> Array.filter(fun filePath -> 
        let ext = Path.GetExtension(filePath)
        (ext=extension))

// Get the first file train data files with .bytes extension
let trainFiles = 
    getFiles "/datadrive/testData" ".bytes"
    |> Array.distinct
    |> Array.length


type DataInput = {
    FileName:string
    Label:string
}

// Create DataInput by using CSV with file name and label
let trainingData (fileName:string) (hasHeader:bool) = 

    let skipRows = 
        match hasHeader with
        | true -> 1
        | false -> 0

    File.ReadAllLines(fileName)
    |> Array.skip skipRows
    |> Array.map(fun line -> 
        let cols = line.Split(',')
        {
            FileName=(sprintf "/datadrive/%s.bytes" cols.[0])
            Label=cols.[1]
        }
    )

let trainData = trainingData "/datadrive/trainLabels.csv" true

let extensions = 
    Directory.GetFiles("/datadrive/trainData")
    |> Array.map(fun filePath -> filePath.Split(".").[1])
    |> Array.distinct

let getFiles directory extension = 
    Directory.GetFiles(directory)
    |> Array.filter(fun filePath -> Path.GetExtension(filePath) = extension)
    
getFiles "/datadrive/trainData" ".asm" |> Array.length
