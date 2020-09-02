open System
open System.IO

[<CLIMutable>]
type ImageData = {
    Id: string
    ImagePath:string
    Label:string
}

let testPredictions = 
    Directory.GetFiles("/datadrive/testImages")
    |> Array.map(fun filePath -> 
        let imageName = Path.GetFileNameWithoutExtension(filePath)
        {Id=imageName;ImagePath=filePath;Label=""})
