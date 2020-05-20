open System
open System.IO
open System.Drawing

let getFilesBytes (filePath:string) = 
    File.ReadAllBytes filePath |> Array.map float32

let padArrayWithZeros (n:int) (firstArray:float32 array) = 
    Array.zeroCreate<float32> n |> Array.append firstArray

let setColors (vals:float32 array) position a r g b = 
    let a' = vals.[position + a]
    let r' = vals.[position + r]
    let g' = vals.[position + g]
    let b' = vals.[position + b]
    a',r',g',b'

[<EntryPoint>]
let main argv =
    // 1. Load binary

    // Byte[43] 
    // let binaryFileBytes = File.ReadAllBytes("mybinary.txt")
    
    let binaryFileBytes = getFilesBytes "stamina-scalable-deep-learning-whitepaper.pdf"

    let imgWidth = 2048
    let imgHeight = 2048
    let dims = imgWidth * imgHeight * 3
    let pad = dims - binaryFileBytes.Length
    let imagePixels = padArrayWithZeros pad binaryFileBytes
    
    let a,r,g,b = 255,0,0,0

    let dst = new Bitmap(imgHeight,imgWidth)

    for y in 0..2047 do
        for x in 0..2047 do
            let position = y * 2048 + x
            let a',r',g',b' = setColors imagePixels position a r g b
            let pixel = Color.FromArgb((int)a',(int)r',(int)g',(int)b')

            dst.SetPixel(x,y,pixel)

    dst.Save("fileImage.jpg",Imaging.ImageFormat.Jpeg)



    0

    

    
    // 2. Convert to image
    
    
    // 3. Use transfer learning to train a model to classify images
    
    
    // 4. Evaluate model
