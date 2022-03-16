using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Runtime.InteropServices;

namespace cameraImg
{
    class Program
    {
        static void Main(string[] args)
        {
            int vyber = 2;
            string barvy = "rgb/bw";
            VideoCapture img; // Vytvoreni zaznamu z kamery, 1 = Externi USB Webkamera, 0 = Integrovana Webka z NTB, cisla = indexy kamer (zacinajici od 0)   

            Console.WriteLine("Vyber si kameru, kterou chceš sledovat:\n----------------------------------------------");
            Console.WriteLine("Napiš 0 pro Integrovanou webkameru z notebooku\nNapis 1 pro Externí USB Webkameru");
            Console.WriteLine("----------------------------------------------");
            int.TryParse(Console.ReadLine(), out int vyberKamery);

            Console.WriteLine("\nVyber si rozliseni:\n0 - 640x480\n1 - 800x600\n2 - 640x360");
            int.TryParse(Console.ReadLine(), out int rozliseni);

            Console.WriteLine("\nChces provest detekci hran?(0 = NE, jakekoliv ostatni cislo = ANO):");
            int.TryParse(Console.ReadLine(), out int detHran);

            if (detHran == 0) { // uzivatel si muze vybrat typ barev a filtrovani obrazu pokud nechce provest detekci hran
                Console.WriteLine("\nVyber si typ barev:");
                Console.WriteLine("RGB / rgb - barevný obraz");
                Console.WriteLine("BW / bw - černobílý obraz");
                barvy = Console.ReadLine();

                Console.WriteLine("\nVyber si způsob filtrování obrazu:");
                Console.WriteLine("0 = žádné\n1 = gauss\n2 = blur(rozmazáním)\n3 = sharpen (zaostření)");
                int.TryParse(Console.ReadLine(), out vyber);
            }

            img = new VideoCapture(vyberKamery); //zacatek videonahravky

            switch (rozliseni) { // nastaveni rozliseni
                case 0: { //640x480
                        img.SetCaptureProperty(CapProp.FrameWidth, 640);
                        img.SetCaptureProperty(CapProp.FrameHeight, 480);
                        break;
                    }
                case 1: { //800x600
                        img.SetCaptureProperty(CapProp.FrameWidth, 800);
                        img.SetCaptureProperty(CapProp.FrameHeight, 600);
                        break;
                    }
                case 2:{ //640x360
                        img.SetCaptureProperty(CapProp.FrameWidth, 640);
                        img.SetCaptureProperty(CapProp.FrameHeight, 360);
                        break;
                    }
                default:{
                        Console.WriteLine("Vybrali jste si spatne rozliseni. Rozliseni se automaticky nastavi na 640x480."); break;
                }
            }
  
            String win1 = "Webkamera"; // Nazev okna
            CvInvoke.NamedWindow(win1); // Vytvoreni okna s nazem v string win1

            while (true) 
            {
                if (detHran == 0) //podminka pro to, ze uzivatel si nevybral ze chce provest detekci hran
                {
                    Image<Bgr, byte> imgf; // 1 frame
                    if (barvy == "rgb" || barvy == "RGB") // podminka pro barevny obraz
                    {
                        imgf = img.QueryFrame().ToImage<Bgr, Byte>(); // prevedeni videa na img frame
                        img.Retrieve(imgf, 0); //ziskani framu

                        if (vyber == 0)
                        {
                            CvInvoke.Imshow(win1, imgf); //zobrazeni cisteho framu
                            CvInvoke.WaitKey(2); 
                        }
                        else if (vyber == 1)
                        {
                            Image<Bgr, byte> gauss = imgf.SmoothGaussian(3, 3, 28, 32); // gaussovo filtrovani             
                            CvInvoke.Imshow(win1, gauss); // zobrazeni framu s gaussovym filtrem
                            CvInvoke.WaitKey(2);
                        }
                        else if (vyber == 2)
                        {
                            Image<Bgr, byte> blur = imgf.SmoothBlur(3, 3, true); //filtrovani rozmazanim

                            CvInvoke.Imshow(win1, blur); // zobrazeni framu s filtrem
                            CvInvoke.WaitKey(2);
                        }
                        else if (vyber == 3)
                        {
                            Sharpen(imgf, 5, 5, 40, 40, 2); //filtrovani pres vlastni fci pro zaostreni obrazu

                            CvInvoke.Imshow(win1, imgf); //zobrazeni framu s zaostrenim
                            CvInvoke.WaitKey(2);
                        }
                    }
                    if (barvy == "bw" || barvy == "BW") // podminka pro cernobily obraz
                    {
                        Mat dst = new Mat();
                        Mat gray = new Mat();

                        imgf = img.QueryFrame().ToImage<Bgr, byte>();
                        img.Retrieve(imgf, 0);                          // ziskani framu z webkamery 

                        if (vyber == 0)
                        {
                            CvInvoke.BilateralFilter(imgf, dst, 0, 10, 2);
                            CvInvoke.CvtColor(dst, gray, ColorConversion.Bgra2Gray); //prevod na cernobily obraz
                            CvInvoke.EqualizeHist(gray, gray); // zlepseni kontrastu                

                            CvInvoke.Imshow(win1, gray); // zobrazeni framu 
                            CvInvoke.WaitKey(2);
                        }
                        else if (vyber == 1)
                        {
                            Image<Bgr, byte> gauss = imgf.SmoothGaussian(3, 3, 40, 55); // gaussovo filtrovani

                            CvInvoke.BilateralFilter(imgf, dst, 0, 10, 2);
                            CvInvoke.CvtColor(dst, gray, ColorConversion.Bgra2Gray); //prevod na cernobily obraz
                            CvInvoke.EqualizeHist(gray, gray); // zlepseni kontrastu                

                            CvInvoke.Imshow(win1, gray); // zobrazeni framu 
                            CvInvoke.WaitKey(2);
                        }
                        else if (vyber == 2)
                        {
                            Image<Bgr, byte> blur = imgf.SmoothBlur(3, 3, true); // filtrovani rozmazanim

                            CvInvoke.BilateralFilter(imgf, dst, 0, 10, 2);
                            CvInvoke.CvtColor(dst, gray, ColorConversion.Bgra2Gray); //prevod na cernobily obraz
                            CvInvoke.EqualizeHist(gray, gray); //zlepseni kontrastu
                         
                            CvInvoke.Imshow(win1, gray); //zobrazeni framu 
                            CvInvoke.WaitKey(2);
                        }
                        else if (vyber == 3)
                        {
                            Sharpen(imgf, 5, 5, 40, 40, 2); // vlastni filtr - filtrovani zaostrenim

                            CvInvoke.BilateralFilter(imgf, dst, 0, 10, 2);
                            CvInvoke.CvtColor(dst, gray, ColorConversion.Bgra2Gray); //prevod na cernobily obraz
                            CvInvoke.EqualizeHist(gray, gray); //zlepseni kontrastu

                            CvInvoke.Imshow(win1, gray); //zobrazeni framu
                            CvInvoke.WaitKey(2);
                        }
                    }
                    else
                    {
                        while (barvy != "rgb" && barvy != "RGB" && barvy != "bw" && barvy != "BW")
                        {
                            Console.WriteLine("Provedli jste špatný výběr. Vyberte si prosím znova z dvou možností. Možnosti lze napsat i malými písmeny.");
                            barvy = Console.ReadLine();
                        }
                    }
                }
                else { // kod ktery provede pouze detekci hran
                    Image<Gray, byte> imgf;
                    Mat dst = new Mat();
                    Mat final = new Mat();

                    img.SetCaptureProperty(CapProp.AutoExposure, 0.25);
                    img.SetCaptureProperty(CapProp.Exposure, -4);   //zlepseni expozice obrazu

                    imgf = img.QueryFrame().ToImage<Gray, byte>(); 
                    img.Retrieve(imgf, 0); //ziskani framu v odstinech sedi

                    CvInvoke.Canny(imgf, dst, 45, 45); // detekce hran v obrazu pres Canny funkci
                    CvInvoke.Threshold(dst, final, 0, 255, ThresholdType.Otsu); //funkce pro threshold z dokumentace od emgucv

                    Thinning(final,final, 0); //fce od EmguCV pro Thinning

                    CvInvoke.Imshow(win1, final); // zobrazeni framu v okne
                    CvInvoke.WaitKey(20);
                    CvInvoke.Imwrite("frame.jpg", final); // zapis vystupu - ve slozce kde je .exe soubor se vytvori soubor frame.jpg a zde se ukladaji prubezne framy z kamery
                }
               
            }
        }
        public static Image<Bgr, byte> Sharpen(Image<Bgr, byte> frame, int sirka, int vyska, double sigma1, double sigma2, int k)
        {
            sirka = (sirka % 2 == 0) ? sirka - 1 : sirka;
            vyska = (vyska % 2 == 0) ? vyska - 1 : vyska;
            
            var gauss_base = frame.SmoothGaussian(sirka, vyska, sigma1, sigma2); // pouziti gaussoveho filtru pomoci sirky, vysky a oběma sigmama
            
            var maska = frame - gauss_base; // vytvoreni masky odectenim framu s gaussovym filtrem od originalniho framu
            
            maska *= k; // Přidání vážené části z masky k původnímu obrazu vynásobením masky (pouze hrany) pomocí proměnné k zlepší okraje

            frame += maska; // pricteni masky k originalnimu framu
            return frame;
        }


        /// <summary> Z dokumentace od EmguCV
        /// Applies a binary blob thinning operation, to achieve a skeletization of the input image. 
        /// The function transforms a binary blob image into a skeletized form using the technique of Zhang-Suen.
        /// </summary>
        /// <param name="src">Source 8-bit single-channel image, containing binary blobs, with blobs having 255 pixel values.</param>
        /// <param name="dst">Destination image of the same size and the same type as src. The function can work in-place.</param>
        /// <param name="thinningType">Value that defines which thinning algorithm should be used.</param>
        public static void Thinning(IInputArray src, IOutputArray dst, ThinningTypes thinningType)
        {
            using (InputArray iaSrc = src.GetInputArray())
            using (OutputArray oaDst = dst.GetOutputArray())
                cveThinning(iaSrc, oaDst, thinningType);
        }

        [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
        private static extern void cveThinning(IntPtr src, IntPtr dst, ThinningTypes thinningType);

        /// <summary>
        /// Thinning type
        /// </summary>
        public enum ThinningTypes
        {
            ZhangSuen = 0,
            GuoHall = 1
        }
    }
}