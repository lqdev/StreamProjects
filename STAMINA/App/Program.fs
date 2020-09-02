open System
open System.IO
open System.Drawing
open FSharp.Collections.ParallelSeq

let getFilesBytes (filePath:string) = 
    File.ReadAllBytes filePath |> Array.map float32

let padArrayWithZeros (n:int) (firstArray:float32 array) = 
    match n with 
    | 0 -> firstArray
    | _ -> Array.zeroCreate<float32> n |> Array.append firstArray
    
let setColors (vals:float32 array) position a r g b = 
    let a' = vals.[position + a]
    let r' = vals.[position + r]
    let g' = vals.[position + g]
    let b' = vals.[position + b]
    a',r',g',b'

let getImageWidth fileBytes = 
    match fileBytes with 
    | x when (x >=0 && x < 10) -> 32
    | x when (x > 10 && x < 30) -> 64
    | x when (x > 30 && x < 60) -> 128
    | x when (x > 60 && x < 100) -> 256
    | x when (x > 100 && x < 200) -> 384
    | x when (x > 200 && x < 100) -> 512
    | x when (x > 1000 && x < 1500) -> 1024
    | x when x > 1500 -> 2048
    | _ -> 2048

let getFiles directory extension = 
    Directory.GetFiles(directory)
    |> Array.filter(fun filePath -> Path.GetExtension(filePath) = extension)

[<EntryPoint>]
let main argv =

    let files = getFiles "/datadrive/testData" ".bytes"

    files
    |> PSeq.iter(fun filePath -> 
    
        let fileName = Path.GetFileNameWithoutExtension filePath
        let binaryFileBytes = getFilesBytes filePath
        
        let imgWidth = getImageWidth binaryFileBytes.Length
        let imgHeight = imgWidth
        let dims = imgWidth * imgHeight * 3
        let pad = max 0 (dims - binaryFileBytes.Length)
        let imagePixels = padArrayWithZeros pad binaryFileBytes
        let iters = imgWidth - 1
    
        let a,r,g,b = 255,0,0,0

        let dst = new Bitmap(imgHeight,imgWidth)
    
        for y in 0..iters do
            for x in 0..iters do
                let position = y * imgWidth + x
                let a',r',g',b' = setColors imagePixels position a r g b
                let pixel = Color.FromArgb((int)a',(int)r',(int)g',(int)b')
    
                dst.SetPixel(x,y,pixel)

        let outFileName = sprintf "/datadrive/testImages/%s.jpg" fileName
        dst.Save(outFileName, Imaging.ImageFormat.Jpeg)
        printfn "processed %s" fileName
    )

    0