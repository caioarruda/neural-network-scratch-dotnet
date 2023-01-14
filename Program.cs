using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
namespace NeuralNetwork
{
    class Program
    {
        
        static void Main(string[] args)
        {
            int[] layers = new[] { 1024, 20, 1 };
            var nn = new NeuralNetwork(layers)
            {
                Iterations = 1000,              
                Alpha = 0.1,                    
                L2_Regularization = false,       
                Lambda = 0.003,                
                Rnd = new Random(999999999)         
            };
            int qtd = 16;
            var training = new double[16][];
            for(int k = 0; k < qtd; k++){                
                MagickImage img = new MagickImage(@"./images/" + ( k + 1 ).ToString() + ".jpg");
                img.ColorType = ColorType.Bilevel;
                List<IPixel<byte>> pixels = img.GetPixels().ToList();
                int x = 0;
                training[k] = new double[(pixels.Count)+1];
                pixels.ForEach(a => {
                    var color = a.ToColor();
                    training[k][x] = Convert.ToInt32(color.R) != 0 ? 1 : 0;
                    x++;
                });
                if(k < training.Length / 2){
                    training[k][x] = 0;
                }else{
                    training[k][x] = 1;
                }
            }
            //Normalize the input to -1,+1 ?
            //We want a 0 mean, and 1 stdev.


            //Take the first 2 columns as input, and last 1 column as target y (the expected label)
            var input = new double[training.GetLength(0)][];
            for (int i = 0; i < training.GetLength(0); i++)
            {
                input[i] = new double[layers[0]];
                for (int j = 0; j < layers[0]; j++)
                    input[i][j] = training[i][j];
            }
                
            //Create the expected label array
            var y = new double[training.GetLength(0)];
            for (int i = 0; i < training.GetLength(0); i++)
                y[i] = training[i][layers[0]];


            string[] list = new string[4];
            list[0] = "./data/circulo.jpg";
            list[1] = "./data/elipse.jpg";
            list[2] = "./data/quadrado.jpg";
            list[3] = "./data/triangulo.jpg";
            var samples = new double[4][];
            int s = 0;
            foreach (var item in list)
            {
                MagickImage imgChk = new MagickImage(item);
                imgChk.ColorType = ColorType.Bilevel;
                List<IPixel<byte>> pixelsChk = imgChk.GetPixels().ToList();
                double[] check = new double[pixelsChk.Count];
                int ic = 0;
                pixelsChk.ForEach(a => {
                    var color = a.ToColor();
                    check[ic] = Convert.ToInt32(color.R) != 0 ? 1 : 0;
                    ic++;
                });
                samples[s] = check;
                s++;
            }
            nn.Monitor = delegate(TrainingTelemetry t)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.WriteLine($"Iteration {t.Iteration}");


                Console.WriteLine($"{nn.Predict(training[0])[0]} -> 0"); 
                Console.WriteLine($"{nn.Predict(training[1])[0]} -> 0");
                Console.WriteLine($"{nn.Predict(training[8])[0]} -> 1");
                Console.WriteLine($"{nn.Predict(training[9])[0]} -> 1");

                // Console.WriteLine("\nWeights:");
                // for (int l = 0; l < t.Weights.Length; l++)
                // {
                //     Console.WriteLine($"  Layer {l}");
                //     Console.WriteLine("  --------------------------");
                //     for (int j = 0; j < t.Weights[l].GetLength(0); j++)
                //     {
                //         for (int k = 0; k < t.Weights[l].GetLength(1); k++)
                //             Console.Write("  {0:#.##}\t", t.Weights[l][j, k]);

                //         Console.WriteLine();
                //     }
                // }

                // Console.WriteLine("\nBiases:");
                // Console.WriteLine("--------------------------");
                // for (int l = 1; l < t.Bias.Length; l++)
                // {
                //     for (int n = 0; n < t.Bias[l].Length; n++)
                //         Console.Write("  {0:#.##}\t", t.Bias[l][n]);
                //     Console.WriteLine();
                // }

                var absCost = (double)t.Error.Sum(v => Math.Abs(v)) / t.Error.Length;
                Console.WriteLine("\nError {0:#.#####}", absCost);

                //Console.ReadKey(true);
            };

            nn.Train(input, y);

            int it = 0;
            foreach (var item in samples)
            {
                Console.WriteLine($"The network learned if image is circle = {nn.Predict(item)[0]} - Image: " + list[it]);   
                it++;
            }

            Console.WriteLine("press any key to continue");
            Console.ReadKey(true);
        }

        static void GenImages(string[] args)
        {
            for(int i = 0; i < 100; i++){
                MagickImage img = new MagickImage(new MagickColor(255, 255, 255), 600, 600);
                Random r = new Random();
                DrawableStrokeColor strokeColor = new DrawableStrokeColor(new MagickColor("black"));
                DrawableStrokeWidth stokeWidth = new DrawableStrokeWidth(5);
                DrawableFillColor fillColor = new DrawableFillColor(MagickColor.FromRgb(255,255,255));
                var obj = new DrawableRectangle(r.Next(1,599), r.Next(1,599), r.Next(1,599), r.Next(1,599));

                img.Draw(strokeColor, stokeWidth, fillColor, obj);
                img.Write(@"./out/"+(i+1).ToString()+".jpg");
            }
            for(int i = 100; i < 200; i++){
                MagickImage img = new MagickImage(new MagickColor(255, 255, 255), 600, 600);
                Random r = new Random();
                DrawableStrokeColor strokeColor = new DrawableStrokeColor(new MagickColor("black"));
                DrawableStrokeWidth stokeWidth = new DrawableStrokeWidth(5);
                DrawableFillColor fillColor = new DrawableFillColor(MagickColor.FromRgb(255,255,255));
                int x = r.Next(5,595);
                int y = r.Next(5,595);
                int maxpx = 600 - x > x ? x : 600 - x;
                int maxpy = 600 - y > y ? y : 600 - y;
                int max = maxpx > maxpy ? maxpy : maxpx;
                int checkx = x - ( max - r.Next(3, max));
                int checky = y - ( max - r.Next(3, max));
                int perimeterx = maxpx > maxpy ? x : checkx;
                int perimetery = maxpx > maxpy ? checky : y;
                var obj = new DrawableCircle(x,y, perimeterx , perimetery);

                
                img.Draw(strokeColor, stokeWidth, fillColor, obj);
                img.Write(@"./out/"+(i+1).ToString()+".jpg");
            }
        }
    }

}
