open System
open System.IO
open System.Drawing
open Microsoft.ML
open Microsoft.ML.Data
open Microsoft.ML.Transforms


[<CLIMutable>]
type Img = {
    [<VectorType(16384)>]
    Features: byte array
}

[<EntryPoint>]
let main argv =
    // 1. Load binary

    // Byte[43] 
    let binaryFileBytes = File.ReadAllBytes("mybinary.txt")
    
    let imgWidth = 128
    let imgHeight = 128
    let dims = imgWidth * imgHeight
    let pad = dims - binaryFileBytes.Length
    let imgBytes = 
        Array.zeroCreate<byte> pad
        |> Array.append  binaryFileBytes
    


    let ctx = MLContext()
    let idv = ctx.Data.LoadFromEnumerable([|{Features=imgBytes}|])

    let pipeline = 
        EstimatorChain()
            .Append(ctx.Transforms.ConvertToImage(128,128,"Image","Features"))

    let transformed = pipeline.Fit(idv).Transform(idv)

    0

    

    
    // 2. Convert to image
    
    
    // 3. Use transfer learning to train a model to classify images
    
    
    // 4. Evaluate model
