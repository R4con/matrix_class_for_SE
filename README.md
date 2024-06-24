## What is this?
This is the myMatrix Class I wrote for a [Space Engineers](https://www.spaceengineersgame.com) Script.
In Space Engeerins it is possible to write a Script in C# to interact with the Objects ingame. For one of my Scripts (not jet published) I needed to do some Matrix calculations, and the Game does not allow access to a Matrix Libaray. So I wrote my own.

### And what Problem does it solve?
 I initially intented to use it to balance the Center of Mass to specific Point.
 Say you have four Points A,B,C,D where you can add weights to. There is also a "fixed" Center of Mass with a fixed position and weight.
 Then I wanted to know, how much weight does every Point need, so that the Center of Mass of the whole System is on a Point P. P can be any Point, as long as it lays inside the volume enclosed by Points A,B,C,D.

To solve this Problem, we first create a Matrix with all the positions of the variable weights, and a row containing how much weight to be can be applied at that position, in this case 20. Then we create a Vector, which just contains the desired Center of Mass. But, because the fixed weights also have to be taken into account, they have to be substracted from that, together with some scaling factor, which accounts for the weight difference between the fixed and variable Center of Mass.
With that we have a Matrix Equation: dataA = dataB, which can be solved with the implemented Least Square Method. (this is a overdetermined System of Equation, so a spacial Variant has to be used, which is already implemented in .solveLstsq())
```
double[,] dataA = new double[4, 4]; // [3 Coords + 1, 4 different Points]
double[] dataB = new double[4];

dataA = [
    [A.x, B.x, C.x, D.x],
    [A.y, B.y, C.y, D.y],
    [A.z, B.z, C.z, D.z],
    [20  , 20  , 20  , 20],
]

// 2.5 = 50/20 ration between fixed a variable weights
// constMass_CoM.Weight / 50 = number of fixed weights
dataB[0] = P.X - constMass_CoM.X * 2.5 * constMass_CoM.Weight / 50;
dataB[1] = P.Y - constMass_CoM.Y * 2.5 * constMass_CoM.Weight / 50;
dataB[2] = P.Z - constMass_CoM.Z * 2.5 * constMass_CoM.Weight / 50;
dataB[3] = 50;

myMatrix2D DataMatrix = new myMatrix2D(dataA);
double[] SBWeights = DataMatrix.solveLstsq();
```

## What is available in this Library?
My Library offers a Matrix object, which is always a 2D NxM Matrix.
There are most common Matrix Operations available:
- Addition / Substraction
- Scalar Multiplication / Division
- Matrix multiplication
- Transposition
- Inversion
- LUFactorization
- solve / solve (Least Squares) for Linar System of Equations
- Row swapping

There are a few that are missing, as I did not need them for my Script:
- Scalar Addition
- Determinat calculation
If you need them, they should be easy to implement.

## ToDo's
There are multiple flaws of this Library, that I would like to fix in the future. So here is my Small list:
- switch around the the order of the operants in get(x,y) and set(x,y). They are the wrong way around, as it is usually the other way around.
- Check "ToDo's" and other comments in Code

There are probably a few things, which still do not work properly, just because they did not come up during the development of my SE-Script.
When using this library, for anything not covered by my Unit tests, make sure it really does what you expect.