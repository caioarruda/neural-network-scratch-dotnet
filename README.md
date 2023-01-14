# Simple Neural Network 
Simple Neural Network from Scratch for recognize simple 32x32 pixels images in dotnet 

#Prerequisites
### Dotnet7.0

# To Run:
### Is a console application to Win/Linux 
```
dotnet run
```

# Example of images for recognize
![quadrado](https://github.com/caioarruda/neural-network-scratch-dotnet/blob/main/data/1.jpg?raw=true)

#Params to modify and increase accuracy or training time
```
int[] layers = new[] { 2, 2, 1 };
var nn = new NeuralNetwork(layers)
{
    Iterations = 1000,              //training iterations
    Alpha = 3.5,                    //learning rate, lower is slower, too high may not converge.
    L2_Regularization = true,       //set L2 regularization to prevent overfitting
    Lambda = 0.0003,                //strength of L2
    Rnd = new Random(12345)         //provide a seed for repeatable outputs
};
```

# Clone and modified from:
https://github.com/snives/SimpleNeuralNetwork
# Credits to:
https://github.com/snives

