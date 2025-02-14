using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SEPARATE
{
    public partial class F : Form
    {
        /*　平面での急停止　…　平面での急停止を行った際に過電流がどれくらい発生するのかをシミュレーションする。過電流はアーマチュア電圧の操作によって、抑える　*/
        const int Sime_size = 1000; 
        double[] IAarm = new double[Sime_size]; /*アーマチュア電流*/
        double[] G_Hyasa = new double[Sime_size]; /*秒速*/
        double[] G_any = new double[Sime_size];
        double[] G_STOP_rate = new double[Sime_size];

        /* 登坂での急停止　…　上り坂での最高速度での走行時から急停止をかけた時に大電流が発生するかどうかをシミュレーションする。角度による重力加速度を考える　*/
        const int Sime_size_t = 500;
        double[] IAarmt = new double[Sime_size_t]; /*アーマチュア電流*/
        double[] G_Hyasat = new double[Sime_size_t]; /*秒速*/
        double[] G_anyt = new double[Sime_size_t];
        double[] G_STOP_ratet = new double[Sime_size_t];

        /* 下り坂での急停止　…　下り坂での最高速度での走行時から急停止をかけた時に大電流が発生するかどうかをシミュレーションする。角度による重力加速度を考える　*/
        const int Sime_size_k = 400;
        double[] IAarmk = new double[Sime_size_k]; /*アーマチュア電流*/
        double[] G_Hyasak = new double[Sime_size_k]; /*秒速*/
        double[] G_anyk = new double[Sime_size_k];
        double[] G_STOP_ratek = new double[Sime_size_k];

        /* 平面での急発進 … 停止状態から急発進した時に大電流が発生するかどうかをシミュレーションする　*/
        const int Sime_size_s = 2000;
        double[] IAarms = new double[Sime_size_s];/*アーマチュア電流*/
        double[] G_Hyasas = new double[Sime_size_s];/*秒速*/
        double[] G_anys = new double[Sime_size_s];
        double[] G_START_rates = new double[Sime_size_s];



        public F()
        {
            InitializeComponent();
        }
         
       


        void Do_calc1()
        {
            ///////////////////////////////////////DO_calc定義関連//////////////////////////////////////////////////////////////////////////
            /*共通定義*/
            int i;
            double KT = Convert.ToDouble(label55.Text);/*大電流を流した時のトルク係数*/
            const double R0 = 0.0543; /*等価半径*/
            const double RA = 0.0118; /* アーマチュア抵抗*/
            double angle;
            double θ = Convert.ToDouble(numericUpDown1.Text);
            angle = Math.PI * θ / 180.0;
            const double G = 9.80665;
            double KG = 0.02 ;  /*係数*/
            double MSituryou = 0;  /* 質量 kg */
            double a_k; /*下り坂で最高速度のまま1秒走った時の追加速度分*/
            if(checkBox3.Checked == true)
            {
                a_k = G * Math.Sin(-1 * angle) * 3;
            }
            else
            {
                a_k = 0.0;
            }
            

            /*平面急停止定義*/
            double VG;   /*発電電圧 */
            double IF;  /* フィールド電流*/
            double N;   /*回転数rms */
            double IA;  /*アーマチュア電流 */
            double VA;  /*アーマチュア電圧 */
            double Toruku; /*トルク */
            double Force; /*力 */
            double hayasa = 0;/*秒速_m/s */
            double hayasa2;/*秒速_m/s*/
            double STOP_rate = 12.0; /* どの程度アーマチュア電圧を落としているか*/
            IF = 10; /* */
            VA = 24; /* */
            const double dt = 0.001;
            
            /* 登坂急停止定義 */
            double IFt;
            double VAt;
            double IAt;
            double Nt;
            double Forcet;
            double VGt;
            double Torukut;
            double STOP_rate_t = 9.0;
            double hayasat = 0;
            double hayasat2;
            double a;　//加速度
            IFt = 15.0;
            VAt = 18;
            const double dtt = 0.001; /*　ステップ時間 */

            /*下り坂急停止定義(初速に関しては、下り坂ですでに5ｓ走行後として考える)*/
            double VGk;   /*発電電圧 */
            double IFk;  /* フィールド電流*/
            double Nk;   /*回転数rms */
            double IAk;  /*アーマチュア電流 */
            double VAk;  /*アーマチュア電圧 */
            double Torukuk; /*トルク */
            double Forcek; /*力 */
            double hayasak = 0;/*秒速_m/s */
            double hayasak2;/*秒速_m/s*/
            double STOP_rate_k = 2; /* どの程度アーマチュア電圧を落としているか*/          
            IFk = 14; /* */
            VAk = 24; /* */
            const double dtk = 0.001;

            /*平面時の急発進定義*/
            double VGs;   /*発電電圧 */
            double IFs;  /* フィールド電流*/
            double Ns;   /*回転数rms */
            double IAs;  /*アーマチュア電流 */
            double VAs;  /*アーマチュア電圧 */
            double Torukus; /*トルク */
            double Forces; /*力 */
            double hayasas;/*秒速_m/s */
            double hayasas2;/*秒速_m/s*/
            double Start_rate_s = 0; /* どの程度アーマチュア電圧をあげているか*/
            hayasas = 0;　 /* 初速は0km/hとする */
            IFs = 10; /* */
            VAs = 0; /* */
            const double dts = 0.001;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /*質量と速度に関して*/
            /*速度hayasakに関しては、下り坂であるため重力がすでにかかっている状態と仮定している。重力がかかってから3秒に設定している。*/
            if (checkBox1.Checked == true)
            {
                switch (comboBox3.SelectedIndex)
                {
                    case 0:
                        MSituryou = 770 + 200;
                        hayasa = 2000.0 / 3600.0;
                        hayasat = 2000.0/3600.0 * (63.5 / 171.1);
                        hayasak = 2000.0/3600.0 + a_k;
                        break;
                    case 1:
                        MSituryou = 770 + 200;
                        hayasa = 2000.0 / 3600.0;
                        hayasat = 2000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 2000.0 / 3600.0 + a_k;
                        break;
                    case 2:
                        MSituryou = 1420 + 250;
                        hayasa = 2000.0/ 3600.0;
                        hayasat = 2000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 2000.0 / 3600.0 + a_k;
                        break;
                    case 3:
                        MSituryou = 1620 + 250; 
                        hayasa = 2000.0 / 3600.0;
                        hayasat = 2000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 2000.0 / 3600.0 + a_k;
                        break;
                    case 4:
                        MSituryou = 740 + 200;
                        hayasa = 3000.0 / 3600.0;
                        hayasat = 3000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 3000.0 / 3600.0 + a_k;
                        break;
                    case 5:
                        MSituryou = 740 + 200;
                        hayasa = 3000.0 / 3600.0;
                        hayasat = 3000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 3000.0 / 3600.0 + a_k;
                        break;
                    case 6:
                        MSituryou = 1230 + 250;
                        hayasa = 4000.0 / 3600.0;
                        hayasat = 4000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 4000.0 / 3600.0 + a_k;
                        break;
                    case 7:
                        MSituryou = 2280 + 250;
                        hayasa = 3200.0 / 3600.0;
                        hayasat = 3200.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 3200.0 / 3600.0 + a_k;
                        break;
                    case 8:
                        MSituryou = 2300 + 320;
                        hayasa = 3500.0 / 3600.0;
                        hayasat = 3500.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 3500.0 / 3600.0 + a_k;
                        break;
                    default:
                        Console.WriteLine("製品名を選択してください");
                        break;
                }
            }
            else
            {
                switch (comboBox3.SelectedIndex)
                {
                    case 0:
                        MSituryou = 770;
                        hayasa = 2000.0 / 3600.0;
                        hayasat = 2000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 2000.0 / 3600.0 + a_k;
                        break;
                    case 1:
                        MSituryou = 770;
                        hayasa = 2000.0 / 3600.0;
                        hayasat = 2000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 2000.0 / 3600.0 + a_k;
                        break;
                    case 2:
                        MSituryou = 1420;
                        hayasa = 2000.0 / 3600.0;
                        hayasat = 2000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 2000.0 / 3600.0 + a_k;
                        break;
                    case 3:
                        MSituryou = 1620;
                        hayasa = 2000.0 / 3600.0;
                        hayasat = 2000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 2000.0/ 3600.0 + a_k;
                        break;
                    case 4:
                        MSituryou = 740;
                        hayasa = 3000.0 / 3600.0;
                        hayasat = 3000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 3000.0 / 3600.0 + a_k;
                        break;
                    case 5:
                        MSituryou = 740;
                        hayasa = 3000.0 / 3600.0;
                        hayasat = 3000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 3000.0 / 3600.0 + a_k;
                        break;
                    case 6:
                        MSituryou = 1230;
                        hayasa = 4000.0 / 3600.0;
                        hayasat = 4000.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 4000.0 / 3600.0 + a_k;
                        break;
                    case 7:
                        MSituryou = 2280;
                        hayasa = 3200.0 / 3600.0;
                        hayasat = 3200.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 3200.0 / 3600.0 + a_k;
                        break;
                    case 8:
                        MSituryou = 2300;
                        hayasa = 3500.0 / 3600.0;
                        hayasat = 2500.0 / 3600.0 * (63.5 / 171.1);
                        hayasak = 3500.0 / 3600.0 + a_k;
                        break;
                    default:
                        Console.WriteLine("製品名を選択してください");
                        break;
                }
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            /* 平面時での急発進に関する計算 */
            for (i = 0; i < Sime_size_s; i ++)
            {
                G_Hyasas[i] = hayasas * 1000;

                Ns = (hayasas * 60.0) / (2.0 * Math.PI * R0);

                VGs = KG * IFs * Ns;

                IAs = (VAs - VGs) / RA;
 
                IAarms[i] = IAs;

                Torukus = KT * IFs * IAs;


                Forces = Torukus / R0;
                 
                if( hayasas < 0.97222222 )
                {
                    hayasas2 = hayasas + Forces / MSituryou * dts;   /*次のstep*/
                    hayasas = hayasas2;
                }

                if( IAs < 170 )
                {
                    if( 12 > Start_rate_s )
                    {
                        ++ Start_rate_s;
                    }
                }
                else
                {
                    if( Start_rate_s > 12 )
                    {
                        -- Start_rate_s;
                    }
                }

                G_START_rates[i] = Start_rate_s * 30;
                VAs += 24 * Start_rate_s / 3000;

                if(VAs > 24.0)
                {
                    VAs = 24.0;
                    

                }
                G_anys[i] = VAs * 20;


                /*
                    hayasas2 = hayasas + a * dts;
                    hayasas = hayasas2;
                    
                    a = Forces / MSituryou;
                    Forces = KT * IF * (VA - KG * IF * (hayasas * 60.0) / (2.0 * 3.14 * R0)) / RA / R0
                    
                    hayasas と　VAsは比例関係に位置している。
                　　そのことをプログラムすればよい。しかし、それはシュミレーションといってもあまりに近似的なものになりうる
                 */
            }

            if(checkBox2.Checked == true)
            {
                /* 平面時での急停止に関する計算 */
                for (i = 0; i < Sime_size; i++)
                {
                    G_Hyasa[i] = hayasa * 1000;

                    N = (hayasa * 60.0) / (2.0 * 3.14 * R0);

                    VG = KG * IF * N;

                    IA = (VA - VG) / RA;

                    IAarm[i] = IA;

                    Toruku = KT * IF * IA;

                    Force = Toruku / R0;

                    hayasa2 = hayasa + Force / MSituryou * dt; /*次のstep*/

                    hayasa = hayasa2;


                    if (IA < -170.0)
                    {
                        if (1 < STOP_rate)
                        {
                            --STOP_rate;
                        }
                    }
                    else
                    {
                        if (STOP_rate < 12)
                        {
                            ++STOP_rate;
                        }
                    }


                    G_STOP_rate[i] = STOP_rate * 30;
                    VA -= 24.0 * STOP_rate / 3000;


                    if (VA < 0.0)
                    {
                        VA = 0.0;
                    }
                    G_any[i] = VA * 20;
                }



                /*  登坂時での急停止に関わる算出　*/
                for (i = 0; i < Sime_size_t; i++)
                {
                    G_Hyasat[i] = hayasat * 1000;

                    Nt = (hayasat * 60.0) / (2.0 * 3.14 * R0);

                    VGt = KG * IFt * Nt;

                    IAt = (VAt - VGt) / RA;

                    IAarmt[i] = IAt;

                    Torukut = KT * IFt * IAt;

                    Forcet = Torukut / R0 - MSituryou * G * Math.Sin(angle);

                    a = Forcet / MSituryou;

                    if (hayasat > 0)
                    {
                        hayasat2 = hayasat + a * dtt;  /* 次のstep */
                        hayasat = hayasat2;
                    }


                    if (IAt < -170.0)
                    {
                        if (1 < STOP_rate_t)
                        {
                            --STOP_rate_t;
                        }
                    }
                    else
                    {
                        if (STOP_rate_t < 9.0)

                        {
                            ++STOP_rate_t;
                        }
                    }


                    G_STOP_ratet[i] = STOP_rate_t * 30;
                    VAt -= 18.0 * STOP_rate_t / 3000;

                    if (VAt < 0.0)
                    {
                        VAt = 0.0;
                    }
                    G_anyt[i] = VAt * 20;
                }

                /*下り坂での急停止*/
                for (i = 0; i < Sime_size_k; i++)
                {
                    G_Hyasak[i] = hayasak * 1000;
                    Nk = (hayasak * 60.0) / (2.0 * 3.14 * R0);

                    VGk = KG * IFk * Nk;

                    IAk = (VAk - VGk) / RA;

                    IAarmk[i] = IAk;

                    Torukuk = KT * IFk * IAk;

                    Forcek = Torukuk / R0 + MSituryou * G * Math.Sin(angle);

                    a = Forcek / MSituryou;

                    if (hayasak > 0)
                    {
                        hayasak2 = hayasak + a * dtk;  /* 次のstep */
                        hayasak = hayasak2;
                    }

                    if (IAk < -170.0)
                    {
                        if (1 < STOP_rate_k)
                        {
                            STOP_rate_k -= 1;
                        }
                    }
                    else
                    {
                        if (STOP_rate_k < 12.0)

                        {
                            STOP_rate_k += 1;
                        }
                    }


                    G_STOP_ratek[i] = STOP_rate_k * 30;
                    VAk -= 24.0 * STOP_rate_k / 3000;

                    if (VAk < 0.0)
                    {
                        VAk = 0.0;
                    }
                    G_anyk[i] = VAk * 20;

                }
            }
            else
            {
                /* 平面時での急停止に関する計算 */
                for (i = 0; i < Sime_size; i++)
                {
                    G_Hyasa[i] = hayasa * 1000;

                    N = (hayasa * 60.0) / (2.0 * 3.14 * R0);

                    VG = KG * IF * N;

                    IA = (VA - VG) / RA;

                    IAarm[i] = IA;

                    Toruku = KT * IF * IA;

                    Force = Toruku / R0;

                    hayasa2 = hayasa + Force / MSituryou * dt; /*次のstep*/

                    hayasa = hayasa2;


                    G_STOP_rate[i] = 12 * 30;
                    VA -= 24.0 * 12 / 3000;


                    if (VA < 0.0)
                    {
                        VA = 0.0;
                    }
                    G_any[i] = VA * 20;
                }



                /*  登坂時での急停止に関わる算出　*/
                for (i = 0; i < Sime_size_t; i++)
                {
                    G_Hyasat[i] = hayasat * 1000;

                    Nt = (hayasat * 60.0) / (2.0 * 3.14 * R0);

                    VGt = KG * IFt * Nt;

                    IAt = (VAt - VGt) / RA;

                    IAarmt[i] = IAt;

                    Torukut = KT * IFt * IAt;

                    Forcet = Torukut / R0 - MSituryou * G * Math.Sin(angle);

                    a = Forcet / MSituryou;

                    if (hayasat > 0)
                    {
                        hayasat2 = hayasat + a * dtt;  /* 次のstep */
                        hayasat = hayasat2;
                    }


                    G_STOP_ratet[i] = 12 * 30;
                    VAt -= 18.0 * 18 / 3000;

                    if (VAt < 0.0)
                    {
                        VAt = 0.0;
                    }
                    G_anyt[i] = VAt * 20;
                }

                /*下り坂での急停止*/
                for (i = 0; i < Sime_size_k; i++)
                {
                    G_Hyasak[i] = hayasak * 1000;
                    Nk = (hayasak * 60.0) / (2.0 * 3.14 * R0);

                    VGk = KG * IFk * Nk;

                    IAk = (VAk - VGk) / RA;

                    IAarmk[i] = IAk;

                    Torukuk = KT * IFk * IAk;

                    Forcek = Torukuk / R0 + MSituryou * G * Math.Sin(angle);

                    a = Forcek / MSituryou;

                    if (hayasak > 0)
                    {
                        hayasak2 = hayasak + a * dtk;  /* 次のstep */
                        hayasak = hayasak2;
                    }

               


                    G_STOP_ratek[i] = 12 * 30;
                    VAk -= 24.0 * 24 / 3000;

                    if (VAk < 0.0)
                    {
                        VAk = 0.0;
                    }
                    G_anyk[i] = VAk * 20;

                }
            }
            
        }
        
       


        

        private void B1_Load(object sender, EventArgs e)
        {


        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }





        private void Printf(string v, double d)
        {
            throw new NotImplementedException();
        }

        private void PointF(string v1, double v2)
        {
            throw new NotImplementedException();
        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }


        /*諸計算*/
        private void button1_Click_1(object sender, EventArgs e)
        {

            string inputText = fbox.Text;  // テキストボックスの入力を取得
            string inputText1 = abox.Text;  // テキストボックスの入力を取得
            string inputText2 = label55.Text;  // テキストボックスの入力を取得

            if (string.IsNullOrEmpty(inputText))  // 空かどうかのチェック
            {
                MessageBox.Show("未入力です");
            }
            else if (decimal.TryParse(inputText, out _))  // 入力が数値のみで構成されているかのチェック
            {

                if (string.IsNullOrEmpty(inputText1))  // 空かどうかのチェック
                {
                    MessageBox.Show("未入力です");
                }
                else if (decimal.TryParse(inputText1, out _))  // 入力が数値のみで構成されているかのチェック
                {
                    if (string.IsNullOrEmpty(inputText2))  // 空かどうかのチェック
                    {
                        MessageBox.Show("未入力です");
                    }
                    else if (decimal.TryParse(inputText2, out _))  // 入力が数値のみで構成されているかのチェック
                    {
                        decimal a, b, c;
                        a = (decimal)Convert.ToDouble(fbox.Text);
                        b = (decimal)Convert.ToDouble(abox.Text);
                        c = (decimal)Convert.ToDouble(label55.Text);

                        /*トルク値の計算*/
                        decimal d;
                        d = a * b * c;
                        label4.Text = d.ToString();

                        /*磁束の計算*/
                        decimal j;
                        j = d / (b * c);
                        zbox.Text = j.ToString();


                        /*全体の入力の計算*/
                        decimal k, l, m;
                        l = (decimal)Convert.ToDouble(favbox.Text);
                        m = (decimal)Convert.ToDouble(textBox3.Text);
                        k = l * b + m * a * a;

                        label63.Text = k.ToString();

                    }
                    else
                    {
                        MessageBox.Show("文字列です");
                    }
                }
                else
                {
                    MessageBox.Show("文字列です");
                }

            }
            else
            {
                MessageBox.Show("文字列です");
            }







        }

        private void fbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void abox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string inputText = nbox.Text;  // テキストボックスの入力を取得
            string inputText1 = zbox.Text;  // テキストボックスの入力を取得
            string inputText2 = label56.Text;  // テキストボックスの入力を取得



            if (string.IsNullOrEmpty(inputText))  // 空かどうかのチェック
            {
                MessageBox.Show("未入力です");
            }
            else if (decimal.TryParse(inputText, out _))  // 入力が数値のみで構成されているかのチェック
            {

                if (string.IsNullOrEmpty(inputText1))  // 空かどうかのチェック
                {
                    MessageBox.Show("未入力です");
                }
                else if (decimal.TryParse(inputText1, out _))  // 入力が数値のみで構成されているかのチェック
                {
                    if (string.IsNullOrEmpty(inputText2))  // 空かどうかのチェック
                    {
                        MessageBox.Show("未入力です");
                    }
                    else if (decimal.TryParse(inputText2, out _))  // 入力が数値のみで構成されているかのチェック
                    {
                        decimal f, g, h;
                        f = (decimal)Convert.ToDouble(nbox.Text);
                        g = (decimal)Convert.ToDouble(zbox.Text);
                        h = (decimal)Convert.ToDouble(label56.Text);
                        
                        /*逆起電力の計算*/
                        decimal i;
                        i = f * g * h;
                        label9.Text = i.ToString();

                        /*全体の出力計算*/
                        decimal j, k, l, pai;
                        j = (decimal)Convert.ToDouble(label4.Text);
                        k = (decimal)Convert.ToDouble(nbox.Text);
                        pai = 3.1415926535897932384626433833M;
                        l = j * (2 * k * pai / 60);
                        label62.Text = l.ToString();



                    }
                    else
                    {
                        MessageBox.Show("文字列です");
                    }
                }
                else
                {
                    MessageBox.Show("文字列です");
                }

            }
            else
            {
                MessageBox.Show("文字列です");
            }



        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string inputText = sfbox.Text;  // テキストボックスの入力を取得
            string inputText1 = dbox.Text;  // テキストボックスの入力を取得
            string inputText2 = frbox.Text;  // テキストボックスの入力を取得
            string inputText3 = rkbox.Text;
            string inputText4 = abox.Text;


            if (string.IsNullOrEmpty(inputText))  // 空かどうかのチェック
            {
                MessageBox.Show("未入力です");
            }
            else if (decimal.TryParse(inputText, out _))  // 入力が数値のみで構成されているかのチェック
            {

                if (string.IsNullOrEmpty(inputText1))  // 空かどうかのチェック
                {
                    MessageBox.Show("未入力です");
                }
                else if (decimal.TryParse(inputText1, out _))  // 入力が数値のみで構成されているかのチェック
                {
                    if (string.IsNullOrEmpty(inputText2))  // 空かどうかのチェック
                    {
                        MessageBox.Show("未入力です");
                    }
                    else if (decimal.TryParse(inputText2, out _))  // 入力が数値のみで構成されているかのチェック
                    {
                        if (string.IsNullOrEmpty(inputText3))  // 空かどうかのチェック
                        {
                            MessageBox.Show("未入力です");
                        }
                        else if (decimal.TryParse(inputText3, out _))  // 入力が数値のみで構成されているかのチェック
                        {
                            if (string.IsNullOrEmpty(inputText4))  // 空かどうかのチェック
                            {
                                MessageBox.Show("未入力です");
                            }
                            else if (decimal.TryParse(inputText4, out _))  // 入力が数値のみで構成されているかのチェック
                            {
                                decimal k, l, m, n, o, b, p;
                                k = (decimal)Convert.ToDouble(sfbox.Text);
                                l = (decimal)Convert.ToDouble(dbox.Text);
                                m = (decimal)Convert.ToDouble(frbox.Text);
                                n = (decimal)Convert.ToDouble(rkbox.Text);
                                b = (decimal)Convert.ToDouble(abox.Text);

                                /*銅損での抵抗値計算*/
                                o = m * (1 + n * (l - k));
                                label18.Text = o.ToString();

                                /*発熱量の計算*/
                                p = b * b * o * 60;
                                label20.Text = p.ToString();
                            }
                            else
                            {
                                MessageBox.Show("文字列です");
                            }
                        }
                        else
                        {
                            MessageBox.Show("文字列です");
                        }
                    }
                    else
                    {
                        MessageBox.Show("文字列です");
                    }
                }
                else
                {
                    MessageBox.Show("文字列です");
                }

            }
            else
            {
                MessageBox.Show("文字列です");
            }






        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void sfbox_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)

        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void label36_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }



        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label44_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label51_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

            string inputText = ffcbox.Text;  // テキストボックスの入力を取得
            string inputText1 = facbox.Text;  // テキストボックスの入力を取得
            string inputText2 = fssbox.Text;  // テキストボックスの入力を取得
            string inputText3 = frrbox.Text;
            string inputText4 = frpbox.Text;
            string inputText5 = frtbox.Text;  // テキストボックスの入力を取得
            string inputText6 = favbox.Text;  // テキストボックスの入力を取得


                if (string.IsNullOrEmpty(inputText))  // 空かどうかのチェック
                {
                    MessageBox.Show("未入力です");
                }

                else if (decimal.TryParse(inputText, out _)) // 入力が数値のみで構成されているかのチェック
                {

                    if (string.IsNullOrEmpty(inputText1))  // 空かどうかのチェック
                    {
                        MessageBox.Show("未入力です");
                    }
                    else if (decimal.TryParse(inputText1
                        , out _))  // 入力が数値のみで構成されているかのチェック
                    {
                        if (string.IsNullOrEmpty(inputText2))  // 空かどうかのチェック
                        {
                            MessageBox.Show("未入力です");
                        }
                        else if (decimal.TryParse(inputText2, out _))  // 入力が数値のみで構成されているかのチェック
                        {
                            if (string.IsNullOrEmpty(inputText3))  // 空かどうかのチェック
                            {
                                MessageBox.Show("未入力です");
                            }
                            else if (decimal.TryParse(inputText3, out _))  // 入力が数値のみで構成されているかのチェック
                            {
                                if (string.IsNullOrEmpty(inputText4))  // 空かどうかのチェック
                                {
                                    MessageBox.Show("未入力です");
                                }
                                else if (decimal.TryParse(inputText4, out _))  // 入力が数値のみで構成されているかのチェック
                                {
                                    if (string.IsNullOrEmpty(inputText5))  // 空かどうかのチェック
                                    {
                                        MessageBox.Show("未入力です");
                                    }
                                    else if (decimal.TryParse(inputText5, out _))  // 入力が数値のみで構成されているかのチェック
                                    {
                                        if (string.IsNullOrEmpty(inputText6))  // 空かどうかのチェック
                                        {
                                            MessageBox.Show("未入力です");
                                        }
                                        else if (decimal.TryParse(inputText6, out _))  // 入力が数値のみで構成されているかのチェック
                                        {

                                            decimal ffc, fac, fss, frr, frt, frp, fav; // 登り時の記録がなく、平面時の記録のみがある場合
                                                                                       // 平面時の値のみを使用して計算する
                                            ffc = (decimal)Convert.ToDouble(ffcbox.Text);
                                            fac = (decimal)Convert.ToDouble(facbox.Text);
                                            fss = (decimal)Convert.ToDouble(fssbox.Text);
                                            frr = (decimal)Convert.ToDouble(frrbox.Text);
                                            frp = (decimal)Convert.ToDouble(frpbox.Text);
                                            frt = (decimal)Convert.ToDouble(frtbox.Text);
                                            fav = (decimal)Convert.ToDouble(favbox.Text);

                                            // トルク定数を求める式
                                            decimal a1;
                                            a1 = frt / (ffc * fac);

                                            label55.Text = a1.ToString();


                                            // 逆起電力定数を求める式
                                            decimal a2;
                                            a2 = (fav - fac * frr) / (ffc * fss);

                                            label56.Text = a2.ToString();

                                        }


                                        else
                                        {
                                            MessageBox.Show("文字列です");
                                        }

                                    }
                                    else
                                    {
                                        MessageBox.Show("文字列です");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("文字列です");
                                }
                            }
                            else
                            {
                                MessageBox.Show("文字列です");
                            }

                        }
                        else
                        {
                            MessageBox.Show("文字列です");
                        }
                    }
                    else
                    {
                        MessageBox.Show("文字列です");
                    }
                }
                else
                {
                    MessageBox.Show("文字列です");
                }
            
            

        }

        private void button6_Click(object sender, EventArgs e)
        {
            decimal i, o, w, s;
            i = (decimal)Convert.ToDouble(label63.Text);
            o = (decimal)Convert.ToDouble(label62.Text);

            /*全体損失の算出*/
            w = i - o;
            label69.Text = w.ToString();

            /*内部発熱量の算出*/
            s = w;
            label25.Text = s.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            /*鉄損の算出*/
            decimal a, b, c;
            a = (decimal)Convert.ToDouble(label25.Text);
            b = (decimal)Convert.ToDouble(label20.Text);

            c = a - b;
            label73.Text = c.ToString();

        }

        private void label74_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click_1(object sender, EventArgs e)
        {

        }

#if true 
        /*未使用の算出部*/
        private void button4_Click(object sender, EventArgs e)
        {
            double a, b, c, d;
            a = (double)Convert.ToDouble(label25.Text);
            b = (double)Convert.ToDouble(textBox6.Text);
            c = (double)Convert.ToDouble(label42.Text);
            d = (double)Convert.ToDouble(textBox5.Text);


            double[] pointX = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };
            double[] pointY = { ((a / 0.444 + b) - d) * Math.Exp(-c * 1) + d, ((2 * a / 0.444 + b) - d) * Math.Exp(-c * 2) + d, ((3 * a / 0.444 + b) - d) * Math.Exp(-c * 3) + d, ((4 * a / 0.444 + b) - d) * Math.Exp(-c * 4) + d, ((5 * a / 0.444 + b) - d) * Math.Exp(-c * 5) + d, ((6 * a / 0.444 + b) - d) * Math.Exp(-c * 6) + d, ((7 * a / 0.444 + b) - d) * Math.Exp(-c * 7) + d, ((8 * a / 0.444 + b) - d) * Math.Exp(-c * 8) + d, ((9 * a / 0.444 + b) - d) * Math.Exp(-c * 9) + d, ((10 * a / 0.444 + b) - d) * Math.Exp(-c * 10) + d, ((11 * a / 0.444 + b) - d) * Math.Exp(-c * 11) + d, ((12 * a / 0.444 + b) - d) * Math.Exp(-c * 12) + d, ((13 * a / 0.444 + b) - d) * Math.Exp(-c * 13) + d, ((14 * a / 0.444 + b) - d) * Math.Exp(-c * 14) + d, ((15 * a / 0.444 + b) - d) * Math.Exp(-c * 15) + d, ((16 * a / 0.444 + b) - d) * Math.Exp(-c * 16) + d, ((17 * a / 0.444 + b) - d) * Math.Exp(-c * 17) + d, ((18 * a / 0.444 + b) - d) * Math.Exp(-c * 18) + d, ((19 * a / 0.444 + b) - d) * Math.Exp(-c * 19) + d, ((20 * a / 0.444 + b) - d) * Math.Exp(-c * 20) + d, ((21 * a / 0.444 + b) - d) * Math.Exp(-c * 21) + d, ((22 * a / 0.444 + b) - d) * Math.Exp(-c * 22) + d, ((23 * a / 0.444 + b) - d) * Math.Exp(-c * 23) + d, ((24 * a / 0.444 + b) - d) * Math.Exp(-c * 24) + d, ((25 * a / 0.444 + b) - d) * Math.Exp(-c * 25) + d, ((26 * a / 0.444 + b) - d) * Math.Exp(-c * 26) + d, ((27 * a / 0.444 + b) - d) * Math.Exp(-c * 27) + d, ((28 * a / 0.444 + b) - d) * Math.Exp(-c * 28) + d, ((29 * a / 0.444 + b) - d) * Math.Exp(-c * 29) + b, ((30 * (a / 0.444 + b)) - d) * Math.Exp(-c * 30) + d, ((31 * a / 0.444 + b) - d) * Math.Exp(-c * 31) + d, ((32 * a / 0.444 + b) - d) * Math.Exp(-c * 32) + d, ((33 * a / 0.444 + b) - d) * Math.Exp(-c * 33) + d, ((34 * a / 0.444 + b) - d) * Math.Exp(-c * 34) + b, ((35 * a / 0.444 + b) - d) * Math.Exp(-c * 35) + d, ((36 * a / 0.444 + b) - d) * Math.Exp(-c * 36) + d, ((37 * a / 0.444 + b) - d) * Math.Exp(-c * 37) + d, ((38 * a / 0.444 + b) - d) * Math.Exp(-c * 38) + d, ((39 * a / 0.444 + b) - d) * Math.Exp(-c * 39) + d, ((40 * a / 0.444 + b) - d) * Math.Exp(-c * 40) + d, ((41 * a / 0.444 + b) - d) * Math.Exp(-c * 41) + d, ((42 * a / 0.444 + b) - d) * Math.Exp(-c * 42) + d, ((43 * a / 0.444 + b) - d) * Math.Exp(-c * 43) + d, ((44 * (a / 0.444 + b)) - d) * Math.Exp(-c * 44) + d, ((45 * a / 0.444 + b) - d) * Math.Exp(-c * 45) + d, ((46 * a / 0.444 + b) - d) * Math.Exp(-c * 46) + d, ((47 * a / 0.444 + b) - d) * Math.Exp(-c * 47) + d, ((48 * a / 0.444 + b) - d) * Math.Exp(-c * 48) + d, ((49 * a / 0.444 + b) - d) * Math.Exp(-c * 49) + d, ((50 * a / 0.444 + b) - d) * Math.Exp(-c * 50) + d };

        }
#endif

        /*未使用*/
        /*伝熱定数を算出するための計算式*/
        private void button8_Click(object sender, EventArgs e)
        {
            string inputText = textBox1.Text;    // テキストボックスの入力を取得
            string inputText1 = textBox4.Text;  // テキストボックスの入力を取得
            string inputText2 = textBox5.Text;  // テキストボックスの入力を取得
            string inputText3 = textBox2.Text;


            if (string.IsNullOrEmpty(inputText))  // 空かどうかのチェック
            {
                MessageBox.Show("未入力です");
            }
            else if (decimal.TryParse(inputText, out _))  // 入力が数値のみで構成されているかのチェック
            {

                if (string.IsNullOrEmpty(inputText1))  // 空かどうかのチェック
                {
                    MessageBox.Show("未入力です");
                }
                else if (decimal.TryParse(inputText1, out _))  // 入力が数値のみで構成されているかのチェック
                {
                    if (string.IsNullOrEmpty(inputText2))  // 空かどうかのチェック
                    {
                        MessageBox.Show("未入力です");
                    }
                    else if (decimal.TryParse(inputText2, out _))  // 入力が数値のみで構成されているかのチェック
                    {
                        if (string.IsNullOrEmpty(inputText3))  // 空かどうかのチェック
                        {
                            MessageBox.Show("未入力です");
                        }
                        else if (decimal.TryParse(inputText3, out _))  // 入力が数値のみで構成されているかのチェック
                        {
                            double a, b, c, d, f;
                            a = double.Parse(inputText);
                            b = double.Parse(inputText1);
                            c = double.Parse(inputText2);
                            d = double.Parse(inputText3);

                            f = (-1 / d) * Math.Log((b - c) / (a - c));
                            label42.Text = f.ToString();
                        }
                        else
                        {
                            MessageBox.Show("文字列です");
                        }



                    }
                    else
                    {
                        MessageBox.Show("文字列です");
                    }
                }
                else
                {
                    MessageBox.Show("文字列です");
                }

            }
            else
            {
                MessageBox.Show("文字列です");
            }
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

       
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void スタートから平常_Click(object sender, EventArgs e)
        {

        }

        private void label45_Click(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }


        ///////////////////////////////////////////////////////////////////*グラフ表示用*////////////////////////////////////////////////////////////////////////////////
        private void button9_Click_1(object sender, EventArgs e)
        {
　　　　　　if(comboBox1.SelectedIndex == 0 )
            {
               
                Do_calc1();

                chart1.ChartAreas.Clear();
                chart1.Series.Clear();

                ChartArea chartA = new ChartArea("data1");
                chart1.ChartAreas.Add(chartA);

                Series oresen1 = new Series();
                oresen1.ChartType = SeriesChartType.Line;
                oresen1.LegendText = "IAarm";

                Series oresen2 = new Series();
                oresen2.ChartType = SeriesChartType.Line;
                oresen2.LegendText = "G_Hyasa (x1000)";

                Series oresen3 = new Series();
                oresen3.ChartType = SeriesChartType.Line;
                oresen3.LegendText = "G_any (x20)";

                Series oresen4 = new Series();
                oresen4.ChartType = SeriesChartType.Line;
                oresen4.LegendText = "G_STOP_rate (x30)";

                for (int i = 0; i < Sime_size; i++)
                {

                    oresen1.Points.AddXY(i, IAarm[i]);

                    oresen2.Points.AddXY(i, G_Hyasa[i]);

                    oresen3.Points.AddXY(i, G_any[i]);

                    oresen4.Points.AddXY(i, G_STOP_rate[i]);


                }


                chart1.Series.Add(oresen1);
                chart1.Series.Add(oresen2);
                chart1.Series.Add(oresen3);
                chart1.Series.Add(oresen4);
            }

            /*角度が1以上であるとき*/
            else if(comboBox1.SelectedIndex == 1)
            {
                
                Do_calc1();

                chart1.ChartAreas.Clear();
                chart1.Series.Clear();

                ChartArea chartA = new ChartArea("data1");
                chart1.ChartAreas.Add(chartA);

                Series oresen1 = new Series();
                oresen1.ChartType = SeriesChartType.Line;
                oresen1.LegendText = "IAarmt";

                Series oresen2 = new Series();
                oresen2.ChartType = SeriesChartType.Line;
                oresen2.LegendText = "G_Hyasat(x1000)";

                Series oresen3 = new Series();
                oresen3.ChartType = SeriesChartType.Line;
                oresen3.LegendText = "G_anyt(x20)";

                Series oresen4 = new Series();
                oresen4.ChartType = SeriesChartType.Line;
                oresen4.LegendText = "G_STOP_ratet(x30)";

                for (int i = 0; i < Sime_size_t; i++)
                {

                    oresen1.Points.AddXY(i, IAarmt[i]);

                    oresen2.Points.AddXY(i, G_Hyasat[i]);

                    oresen3.Points.AddXY(i, G_anyt[i]);

                    oresen4.Points.AddXY(i, G_STOP_ratet[i]);


                }


                chart1.Series.Add(oresen1);
                chart1.Series.Add(oresen2);
                chart1.Series.Add(oresen3);
                chart1.Series.Add(oresen4);
            }
            /*下り坂の時*/
            else if (comboBox1.SelectedIndex == 2)
            {
               
                Do_calc1();

                chart1.ChartAreas.Clear();
                chart1.Series.Clear();

                ChartArea chartA = new ChartArea("data1");
                chart1.ChartAreas.Add(chartA);

                Series oresen1 = new Series();
                oresen1.ChartType = SeriesChartType.Line;
                oresen1.LegendText = "IAarmk";

                Series oresen2 = new Series();
                oresen2.ChartType = SeriesChartType.Line;
                oresen2.LegendText = "G_Hyasak(x1000)";

                Series oresen3 = new Series();
                oresen3.ChartType = SeriesChartType.Line;
                oresen3.LegendText = "G_anyk(x20)";

                Series oresen4 = new Series();
                oresen4.ChartType = SeriesChartType.Line;
                oresen4.LegendText = "G_STOP_ratek(x30)";

                for (int i = 0; i < Sime_size_k; i++)
                {

                    oresen1.Points.AddXY(i, IAarmk[i]);

                    oresen2.Points.AddXY(i, G_Hyasak[i]);

                    oresen3.Points.AddXY(i, G_anyk[i]);

                    oresen4.Points.AddXY(i, G_STOP_ratek[i]);


                }


                chart1.Series.Add(oresen1);
                chart1.Series.Add(oresen2);
                chart1.Series.Add(oresen3);
                chart1.Series.Add(oresen4);
            }
            else
            {
                Do_calc1();

                chart1.ChartAreas.Clear();
                chart1.Series.Clear();

                ChartArea chartA = new ChartArea("data1");
                chart1.ChartAreas.Add(chartA);

                Series oresen1 = new Series();
                oresen1.ChartType = SeriesChartType.Line;
                oresen1.LegendText = "IAarms";

                Series oresen2 = new Series();
                oresen2.ChartType = SeriesChartType.Line;
                oresen2.LegendText = "G_Hyasas";

                Series oresen3 = new Series();
                oresen3.ChartType = SeriesChartType.Line;
                oresen3.LegendText = "G_anys";

                Series oresen4 = new Series();
                oresen4.ChartType = SeriesChartType.Line;
                oresen4.LegendText = "G_START_rates";

                for (int i = 0; i < Sime_size_s; i++)
                {

                    oresen1.Points.AddXY(i, IAarms[i]);

                    oresen2.Points.AddXY(i, G_Hyasas[i]);

                    oresen3.Points.AddXY(i, G_anys[i]);

                    oresen4.Points.AddXY(i, G_START_rates[i]);


                }


                chart1.Series.Add(oresen1);
                chart1.Series.Add(oresen2);
                chart1.Series.Add(oresen3);
                chart1.Series.Add(oresen4);
            }
        }
        ///////////////////////////////////////////////////////////////////**//////////////////////////////////////////////////////////////////////////
       

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void label45_Click_1(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label5_Click_2(object sender, EventArgs e)
        {

        }

       

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void sfbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void label56_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

       

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label55_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                numericUpDown1.Value = 0;
                numericUpDown1.Minimum = 0;
                numericUpDown1.Maximum = 12;
                numericUpDown1.Increment = 1;

            }
            else if(comboBox1.SelectedIndex == 2)
            {
                numericUpDown1.Value = 0;
                numericUpDown1.Minimum = -12;
                numericUpDown1.Maximum = 0;
                numericUpDown1.Increment = 1;
            }
            else
            {
                numericUpDown1.Value = 0;
                numericUpDown1.Minimum = 0;
                numericUpDown1.Maximum = 0;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
             *　　ffc = フィールド電流
             *　　fac = アーマチュア電流
             *　　fss = 回転速度
             *　　frr = アーマチュア抵抗
             *　　frp = 定格出力
             *　　frt = 定格トルク
             *　　fav = アーマチュア電圧
             *　　textBox3 = フィールド抵抗
             */

            /*手動入力の場合*/
            if(comboBox2.SelectedIndex == 2)
            {
                ffcbox.Text = null;    
                facbox.Text = null;
                fssbox.Text = null;
                frrbox.Text = null;
                frpbox.Text = null;
                frtbox.Text = null;
                favbox.Text = null;
                textBox3.Text = null;
            }
            /*仕様書平面の場合*/
            else if(comboBox2.SelectedIndex == 0)
            {
                ffcbox.Text = 6.1.ToString();
                facbox.Text = 36.ToString();
                fssbox.Text = 171.1.ToString();
                frrbox.Text = 0.0118.ToString();
                frpbox.Text = 630.ToString();
                frtbox.Text = 31.7.ToString();
                favbox.Text = 24.ToString();
                textBox3.Text = 0.01.ToString();
            }
            /*仕様書登坂の場合*/
            else if(comboBox2.SelectedIndex == 1)
            {
                ffcbox.Text = 16.ToString();
                facbox.Text = 170.ToString();
                fssbox.Text = 63.5.ToString();
                frrbox.Text = 0.0118.ToString();
                frpbox.Text = 2030.ToString();
                frtbox.Text = 275.4.ToString();
                favbox.Text = 18.ToString();
                textBox3.Text = 0.01.ToString();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    
}





      




            
       

       
