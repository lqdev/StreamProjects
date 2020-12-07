#r "nuget:Microsoft.ML"

open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.ML
open Microsoft.ML.Data

[<CLIMutable>]
type ArxivData = {
    [<JsonPropertyName("title")>]
    Title : string
    
    [<JsonPropertyName("abstract")>]
    Abstract : string
    
    [<JsonPropertyName("categories")>]
    Categories : string
}

[<CLIMutable>]
type ArXivPrediction = {
    [<ColumnName("PredictedLabel")>]
    Category: string
    Categories:string
}

let loadData (file:String) = seq {
    use reader = new StreamReader(file)
    while not reader.EndOfStream do
        let line = reader.ReadLine()
        yield JsonSerializer.Deserialize<ArxivData>(line)
}


let trainFile = "/datadrive/Data/ArXivTrainData/train.json"

let trainData = loadData trainFile |> Seq.take 10

let ctx = MLContext()

let idv = ctx.Data.LoadFromEnumerable(trainData)

let (model,schema) = ctx.Model.Load("/home/azuser/StreamProjects/ArXivAnalytics/ClassificationModel/arxiv-model.zip")

let scoringPipeline = ctx.Transforms.Conversion.MapKeyToValue("PredictedLabel")

let originalPipelinePredictions = model.Transform(idv)

let scoringModel = scoringPipeline.Fit(originalPipelinePredictions)

let predictions = scoringModel.Transform(originalPipelinePredictions)

let predictionSeq = ctx.Data.CreateEnumerable<ArXivPrediction>(predictions, true)

predictionSeq
|> Seq.iter(fun x -> printfn "Original: %s  | Predicted: %s" x.Categories x.Category) 

let newData = seq {
    {
        Title="Donaldson Functional in Teichmüller Theory"
        Abstract="In this paper we define the Donaldson functional on closed Riemann surfaces whose Euler-Lagrange equations are a system of differential equations which generalizes Hitchin's self-dual equations on closed surfaces. We prove that this functional admits a unique critical point which is a global minimum. An immediate consequence is that this system of generalized self-dual equations admits also a unique solution. Among the applications in geometry of this fact, we find closed minimal immersions with the given Higgs data in hyperbolic manifolds, and consequently we present a unified variational approach to construct representations of the fundamental groups of closed surfaces into several character varieties."
        Categories=""
    }
}

let newIdv = ctx.Data.LoadFromEnumerable(newData)

let newPreds = scoringModel.Transform(model.Transform(newIdv))

let newPredsSeq = ctx.Data.CreateEnumerable<ArXivPrediction>(newPreds,true)

newPredsSeq |> Seq.head