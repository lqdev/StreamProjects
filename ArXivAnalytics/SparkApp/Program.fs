// Learn more about F# at http://fsharp.org

open System
open Microsoft.Spark
open Microsoft.Spark.Sql

let DATA_DIR = "/datadrive/Data/Kaggle/arxiv-metadata-oai-snapshot-2020-08-14.json"

[<EntryPoint>]
let main argv =
    
    let sparkSession = 
        SparkSession
            .Builder()
            .AppName("arxiv-analytics")
            .GetOrCreate()

    let arxivData = 
        sparkSession
            .Read()
            .Json([|DATA_DIR|])
    
    let categories = 
        arxivData
            .Select(Functions.Col("categories").Alias("categories"))
            .GroupBy("categories")
            .Count()
            .OrderBy(Functions.Col("count").Desc())

    categories.Show(10)

    sparkSession.Stop()
    0 // return an integer exit code
