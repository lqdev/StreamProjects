open System.IO


let filePath = Path.Combine(__SOURCE_DIRECTORY__,"mybinary.txt")
printfn "%s" filePath
let binaryFileBytes = File.ReadAllBytes(filePath)

printfn "%A" binaryFile

let imgWidth = 128
let imgHeight = 128
let dims = imgWidth * imgHeight
let pad = dims - binaryFileBytes.Length
let imgBytes = 
    Array.zeroCreate<byte> pad
    |> Array.append  binaryFileBytes

imgBytes.Length


#I "C:/Users/lqdev/.nuget/packages"
#r "system.drawing.common/4.7.0/lib/net461/System.Drawing.Common.dll"

// #r "nuget:System.Drawing.Common"

open System.Drawing

use ms = new MemoryStream(imgBytes)
let img = Image.FromStream(ms)
img.Save("img.jpeg")
