# Static Malware Analysis using Deep Learning

Implementation of [STAMINA (Static Malware-as-Image Network Analysis)](https://www.intel.com/content/dam/www/public/us/en/ai/documents/stamina-scalable-deep-learning-whitepaper.pdf), a technique to categorize malware using deep learning.

Check out the [YouTube playlist](https://www.youtube.com/playlist?list=PLsdMoYmuvh9YxMaIrb7k7Tej4-8LxVheG) containing recordings from the [Twitch stream](https://www.twitch.tv/lqdev1) where this project was implemented.

## Data

The data used for this project comes from the [Kaggle Microsoft Malware Classification Challenge (BIG 2015)](https://www.kaggle.com/c/malware-classification/data)

## Project Structure

- **App**: F# .NET Core Core Console application to encode file bytes as an image.
- **TrainingConsole**: F# .NET Core Console application that uses ML.NET's Image Classification API to train 
- **PredictiConsole**: F# .NET Core Console application that uses the trained model from *TrainingConsole* to make predictions on the test data.

### Flow

App -> TrainingConsole -> PredictionConsole
