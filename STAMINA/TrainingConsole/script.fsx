open System.IO

type ImageData = {
    ImagePath:string
    Label:string
}

let getImageData trainFileName (sep:char) hasHeader dataDir ext = 
    
    let skipRows = 
        match hasHeader with
        | true -> 1
        | false -> 0
    
    File.ReadAllLines trainFileName
    |> Array.skip skipRows
    |> Array.map(fun row -> 
        let (cols:string array) = row.Split(sep)

        let trimmedFileName = cols.[0].Trim('"')

        let imagePath = sprintf "%s/%s.%s" dataDir trimmedFileName ext
        {
            ImagePath=imagePath
            Label=cols.[1]
        }
    )

getImageData "/datadrive/trainLabels.csv" ',' true "/datadrive/trainImages" "jpg"

[<CLIMutable>]
type ImagePrediction = {
    PredictedLabel:string
    Score: single array
}


let imageTestData = 
    Directory.GetFiles("/datadrive/testImages")
    |> Array.map(fun filePath -> 
    {ImagePath=filePath;Label=""})
