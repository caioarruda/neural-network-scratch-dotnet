using System;
using System.Collections;
using System.Collections.Generic;

using System.Threading;
using ImageMagick;
using Color = ImageMagick.ColorRGB;

namespace NeuralNetwork {

    public class ConvolutionProcessor {

        MagickImage map;

        public ConvolutionProcessor(MagickImage bitmap) {
            map = bitmap;
        }


        public MagickImage ComputeWith(Filter filter) {
            var result = new MagickImage(new MagickColor(255,255,255) ,map.Width, map.Height);

            var offset = filter.Size / 2;
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    var colorMap = new Color[filter.Size, filter.Size];

                    for (int filterY = 0; filterY < filter.Size; filterY++) {
                        int pk = (filterY + x - offset <= 0) ? 0 :
                            (filterY + x - offset >= map.Width - 1) ? map.Width - 1 : filterY + x - offset;
                        for (int filterX = 0; filterX < filter.Size; filterX++) {
                            int pl = (filterX + y - offset <= 0) ? 0 :
                                (filterX + y - offset >= map.Height - 1) ? map.Height - 1 : filterX + y - offset;

                            IMagickColor<byte> color = map.GetPixels().GetPixel(pk, pl).ToColor();
                            colorMap[filterY, filterX] = new Color(color);
                        }
                    }

                    result.GetPixels().SetPixel(x, y, (colorMap * filter).ToMagickColor().ToByteArray());
                }
            }

            return result;
        }

        public MagickImage ComputeWith(Filter filter, int threadsCount) {
            if (threadsCount == 0)
                throw new ArgumentException("Thread count shouldn't be zero");

            var result = new MagickImage(new MagickColor(255,255,255) ,map.Width, map.Height);
            var threads = new List<Thread>();

            for (int i = 0, start = 0; i < threadsCount; i++) {
                var size = (map.Width - i + threadsCount - 1) / threadsCount;
                var it = start;

                var thread = new Thread(delegate() {
                    Calculate(it, it + size, filter, result);
                });
                thread.Start();
                threads.Add(thread);

                start += size;
            }

            threads.ForEach(thread => thread.Join());

            return result;
        }

        private void Calculate(int start, int finish, Filter filter, MagickImage result) {
            var offset = filter.Size / 2;
            for (int x = start; x < finish; x++) {
                for (int y = 0; y < map.Height; y++) {
                    var colorMap = new Color[filter.Size, filter.Size];

                    for (int filterY = 0; filterY < filter.Size; filterY++) {
                        int pk = (filterY + x - offset <= 0) ? 0 :
                            (filterY + x - offset >= map.Width - 1) ? map.Width - 1 : filterY + x - offset;

                        for (int filterX = 0; filterX < filter.Size; filterX++) {
                            int pl = (filterX + y - offset <= 0) ? 0 :
                                (filterX + y - offset >= map.Height - 1) ? map.Height - 1 : filterX + y - offset;

                            colorMap[filterY, filterX] = new Color(map.GetPixels().GetPixel(pk, pl).ToColor());
                        }
                    }

                    result.GetPixels().SetPixel(x, y, (colorMap * filter).ToMagickColor().ToByteArray());
                }
            }
        }

    }

}