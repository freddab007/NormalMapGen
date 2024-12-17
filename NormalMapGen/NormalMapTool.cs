using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace NormalMapGenerator
{
    struct PixelData
    {
        //bleu
        public Byte b;
        //vert
        public Byte g;
        //rouge
        public Byte r;
        //alpha
        public Byte a;
    }

    public class NormalMapTool
    {

        #region CODE TRAITEMENT NORMALMAP
        bool IsGenerated = false;
        bool IsLoaded = false;
        string fileName = "";
        Bitmap originalBitmap;
        Bitmap normalMapBitmap;

        //Données de la bitmap de l'image source
        BitmapData source;
        //Données de la bitmap de l'image de destination (vide au départ)
        BitmapData destination;

        //déclaration de tableau multidimensionnels (3 colonnes de 3 lignes) -> tableaux en 2 dimensions

        //Les gradients 
        int[,] kernelX = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
        int[,] kernelY = new int[,] { { 1, 0, -1 }, { 2, 0, -2 }, { 1, 0, -1 } };

        int[,] offsetX = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
        int[,] offsetY = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

        int x = 0, y = 0,
            kx = 0, ky = 0,
            sourceStride = 0, destinationStride = 0,
            sumX = 128, sumY = 128, sumZ = 0;

        private unsafe void GenerateNormalMap()
        {
            //Déclaration "unsafe" sur lesquelles il sera possible d'utiliser des pointeurs en c#
            PixelData* pixelSource;
            PixelData* pixelDestination;

            //Vérrouillage (allocation) mémoire des octets de l'image source (en lecture)
            source =
                originalBitmap.LockBits(
                    new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            //Vérrouillage (allocation) mémoire des octets de l'image de destination (en écriture)
            destination =
                normalMapBitmap.LockBits(
                    new Rectangle(0, 0, normalMapBitmap.Width, normalMapBitmap.Height),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            // On déclare que le pixelData "source" pointe sur le scan0 de l'image source
            pixelSource = (PixelData*)source.Scan0.ToPointer();

            // On déclare que le pixelData de "destination" pointe sur le scan0 de l'image source
            pixelDestination = (PixelData*)destination.Scan0.ToPointer();


            //Définition du Stride d'un Bitmap

            /**
             * Type: System.Int32
             * Largeur de numérisation, en octets, de la bitmap objet.
             * 
             * Cette valeur est la largeur d'une seule ligne de pixels (une ligne de numérisation),
             * arrondie à une limite de 4 octets. Si cette valeur est positive, la Bitmap est de haut en bas.
             * Si cette valeur est négative, la bitmap est de bas en haut.
             * 
             * 
             * Ces pixels pointent sur un "UINT"
             * Le code doit donc convertir le nombre UINT qui correspeond au scan de la ligne.
             * Parce que le UINT est sur 4 octets, on divise par 4 pour avoir la taille du Stride.
             * */
            sourceStride = source.Stride / 4;
            destinationStride = destination.Stride / 4;

            //Ici démarre le traitement de l'image, de haut en bas, de gauche à droite, pixel par pixel.

            //Sur la hauteur de l'image
            for (y = 0; y < source.Height; y++)
            {
                //on définit d'abord les offsetY pour les bords de l'image

                //La première ligne
                if (y == 0)
                {
                    offsetY[0, 0] = (source.Height - 1) * sourceStride; //lecture de bas en haut -> tout en haut à droite
                    offsetY[1, 0] = (source.Height - 1) * sourceStride;
                    offsetY[2, 0] = (source.Height - 1) * sourceStride;
                    offsetY[0, 2] = 0; //lecture de bas en haut -> tout en haut à gauche
                    offsetY[1, 2] = 0;
                    offsetY[2, 2] = 0;
                }

                //la dernière ligne (height -1, car départ index =0
                else if (y == source.Height - 1)
                {
                    offsetY[0, 0] = 0;
                    offsetY[1, 0] = 0;
                    offsetY[2, 0] = 0;
                    offsetY[0, 2] = -((source.Height - 1) * sourceStride);
                    offsetY[1, 2] = -((source.Height - 1) * sourceStride);
                    offsetY[2, 2] = -((source.Height - 1) * sourceStride);
                }

                //l'avant dernière ligne (height -2) et la deuxième ligne
                else if (y == 1 || y == source.Height - 2)
                {
                    offsetY[0, 0] = 0;
                    offsetY[1, 0] = 0;
                    offsetY[2, 0] = 0;
                    offsetY[0, 2] = 0;
                    offsetY[1, 2] = 0;
                    offsetY[2, 2] = 0;
                }

                //sur la largeur
                for (x = 0; x < source.Width; x++)
                {
                    (pixelDestination + (y * destinationStride) + x)->a = 255;
                    //sur la première colonne
                    if (x == 0)
                    {
                        offsetX[0, 0] = source.Width - 1;
                        offsetX[0, 1] = source.Width - 1;
                        offsetX[0, 2] = source.Width - 1;
                        offsetX[2, 0] = 0;
                        offsetX[2, 1] = 0;
                        offsetX[2, 2] = 0;
                    }
                    else if (x == source.Width - 1)
                    {
                        offsetX[0, 0] = 0;
                        offsetX[0, 1] = 0;
                        offsetX[0, 2] = 0;
                        offsetX[2, 0] = -source.Width;
                        offsetX[2, 1] = -source.Width;
                        offsetX[2, 2] = -source.Width;
                    }
                    else if (x == 1 || x == source.Width - 2)
                    {
                        offsetX[0, 0] = 0;
                        offsetX[0, 1] = 0;
                        offsetX[0, 2] = 0;
                        offsetX[2, 0] = 0;
                        offsetX[2, 1] = 0;
                        offsetX[2, 2] = 0;
                    }

                    sumX = 128;
                    sumY = 128;

                    for (kx = -1; kx <= 1; kx++)
                    {
                        for (ky = -1; ky <= 1; ky++)
                        {
                            sumX += CalculatePixel(pixelSource, sourceStride, kernelX, kx, ky, x, y);
                            sumY += CalculatePixel(pixelSource, sourceStride, kernelY, kx, ky, x, y);
                        }

                    }

                    //en fonction de la valeur de sumX, on réajuste en cas de dépassement , pour le rouge ( 0 = aucun, 255 = max) , sinon -> valeur réelle
                    if (sumX < 0)
                    {
                        (pixelDestination + (y * destinationStride) + x)->r = 0;
                    }
                    else if (sumX > 255)
                    {
                        (pixelDestination + (y * destinationStride) + x)->r = 255;
                    }
                    else
                    {
                        (pixelDestination + (y * destinationStride) + x)->r = (Byte)sumX;
                    }

                    //en fonction de la valeur de sumY, on réajuste en cas de dépassement , pour le vert ( 0 = aucun, 255 = max) , sinon -> valeur réelle
                    if (sumY < 0)
                    {
                        (pixelDestination + (y * destinationStride) + x)->g = 0;
                    }
                    else if (sumY > 255)
                    {
                        (pixelDestination + (y * destinationStride) + x)->g = 255;
                    }
                    else
                    {
                        (pixelDestination + (y * destinationStride) + x)->g = (Byte)sumY;
                    }

                    //on ajuste le bleu , valeur "milieu" choisie arbitrairement 256/2 = 128
                    sumZ = ((Math.Abs(sumX - 128) + Math.Abs(sumY - 128)) / 4);
                    if (sumZ < 0) sumZ = 0;
                    if (sumZ > 64) sumZ = 64;

                    (pixelDestination + (y * destinationStride) + x)->b = (Byte)(255 - (Byte)sumZ);

                }
            }
            //On libère les ressources mémoire (pour éviter les fuites)
            originalBitmap.UnlockBits(source);
            normalMapBitmap.UnlockBits(destination);

        }

        unsafe int CalculatePixel(PixelData* pixelSource, int sourceStride, int[,] kernelArray, int kx, int ky, int x, int y)
        {
            return kernelArray[kx + 1, ky + 1] *
                ((
                (pixelSource + (y * sourceStride) + x + (ky * sourceStride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->r +
                (pixelSource + (y * sourceStride) + x + (ky * sourceStride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->g +
                (pixelSource + (y * sourceStride) + x + (ky * sourceStride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->b
                ) / 3);
        }

        #endregion

    }
}

