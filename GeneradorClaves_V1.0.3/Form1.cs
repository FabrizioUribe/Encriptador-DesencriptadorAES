using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GeneradorClaves_V1._0._3
{
    public partial class Form1 : Form
    {
        public static bool bandera = false;
        public Form1()
        {
            InitializeComponent();
        }



        public string ValidarOpcionesEncriptar()
        {

            int cm = cmbOpciones.SelectedIndex + 1;
            string result = "", result2 = "";
            switch (cm)
            {
                case 1:
                    result = EncriptarUsuarios(txtCadena.Text);
                    result2 = EncriptarUsuarios(result);
                    break;
                case 2: result2 = CifrarTextoAES(txtCadena.Text, "SHA1", 256, 0); break;
                case 3: result2 = CifrarTextoAES(txtCadena.Text, "SHA1", 256, 50); break;
            }
            return result2;
        }
        public string ValidarOpcionesDesencriptar()
        {
            int cm = cmbOpciones.SelectedIndex + 1;
            string result = "", result2 = "";

            string data = txtCadena.Text.ToString();
            switch (cm)
            {
                case 1:
                    result = DesEncriptarUsuarios(txtCadena.Text);
                    result2 = DesEncriptarUsuarios(result);
                    break;
                case 2: result2 = DescifrarTextoAES(txtCadena.Text, "SHA1", 256, 0); break;
                case 3: result2 = DescifrarTextoAES(txtCadena.Text, "SHA1", 256, 50); break;
            }
            return result2;
        }
        private void btnEncriptar_Click(object sender, EventArgs e)
        {
            ValidarCadena();
            if (!bandera)
            {
                txtMostrar.Text = ValidarOpcionesEncriptar();
                MessageBox.Show("Se ha encriptado la cadena de conexion");
            }
        }

        private void btnDesencriptar_Click(object sender, EventArgs e)
        {
            ValidarCadena();

            if (!bandera)
            {
                //txtMostrar.Text = DescifrarTextoAES(txtCadena.Text, "SHA1", 256, 50);
                txtMostrar.Text = ValidarOpcionesDesencriptar();
                MessageBox.Show("Se ha desencriptado la cadena de conexion");
            }
        }
        public bool ValidarCadena()
        {
            if (txtCadena.Text == null || txtCadena.Text == "")
            {
                MessageBox.Show("Debe Ingresar una Cadena de texto");
                bandera = true;
            }
            else
            {
                bandera = false;
            }
            return bandera;
        }
        private static string EncriptarUsuarios(string _cadenaAencriptar)
        {
            try
            {
                if (_cadenaAencriptar == "")
                {
                    return "";
                }

                string result = string.Empty;
                byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaAencriptar);
                result = Convert.ToBase64String(encryted);
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        private static string DesEncriptarUsuarios(string _cadenaAdesencriptar)
        {
            try
            {
                string result = string.Empty;
                byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
                //result = System.Text.Encoding.Unicode.GetString(decryted, 0, decryted.ToArray().Length);
                result = System.Text.Encoding.Unicode.GetString(decryted);
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        private string CifrarTextoAES(string textoCifrar, string algoritmoEncriptacionHASH, int tamanoClave, int desc)
        {
            try
            {
                string palabraPaso = "c13l03l3naf32r3l44ndr34l3n1nr0b1ns0n";
                string valorRGBSalt = desc == 0 ? "r4m1r3z1ng11ng44g21l4rr4m1r3zc0ll4z0s" : "r4m1r3z1ng11ng44" + desc + "21l4rr4m1r3zc0ll4z0s";
                string vectorInicial = "32st4q214j4c1nt0";

                int iteraciones = 50;
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(vectorInicial);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(valorRGBSalt);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(textoCifrar);

                PasswordDeriveBytes password = new PasswordDeriveBytes(palabraPaso, saltValueBytes, algoritmoEncriptacionHASH, iteraciones);

                byte[] keyBytes = password.GetBytes(tamanoClave / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();


                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, InitialVectorBytes);

                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                string textoCifradoFinal = Convert.ToBase64String(cipherTextBytes);
                return textoCifradoFinal;

            }
            catch (Exception ex)
            {
                return "";
            }
        }
        private string DescifrarTextoAES(string textoCifrado, string algoritmoEncriptacionHASH, int tamanoClave, int desc)
        {
            try
            {
                string palabraPaso = "c13l03l3naf32r3l44ndr34l3n1nr0b1ns0n";
                string valorRGBSalt = "r4m1r3z1ng11ng44" + desc + "21l4rr4m1r3zc0ll4z0s";
                string vectorInicial = "32st4q214j4c1nt0";
                int iteraciones = 50;
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(vectorInicial);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(valorRGBSalt);

                byte[] cipherTextBytes = Convert.FromBase64String(textoCifrado);

                PasswordDeriveBytes password =
                    new PasswordDeriveBytes(palabraPaso, saltValueBytes,
                        algoritmoEncriptacionHASH, iteraciones);

                byte[] keyBytes = password.GetBytes(tamanoClave / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged();

                symmetricKey.Mode = CipherMode.CBC;

                if ((InitialVectorBytes.Length == 0))
                    symmetricKey.Mode = CipherMode.ECB;
                else
                    symmetricKey.Mode = CipherMode.CBC;


                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, InitialVectorBytes);

                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                memoryStream.Close();
                cryptoStream.Close();

                string textoDescifradoFinal = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                return textoDescifradoFinal;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public class ListarOpciones
        {
            public int Id { get; set; }
            public string valor { get; set; }

            public ListarOpciones(int _id, string _valor)
            {
                Id = _id;
                valor = _valor;
            }
            public override string ToString()
            {
                return valor;
            }
        }

        private void FormLoad(object sender, EventArgs e)
        {
            txtMostrar.ReadOnly = true;
            cmbOpciones.Items.Add(new ListarOpciones(1, "Claves de usuario"));
            cmbOpciones.Items.Add(new ListarOpciones(2, "Licencia de dispositivos"));
            cmbOpciones.Items.Add(new ListarOpciones(3, "Cadena de Conexión"));
            cmbOpciones.SelectedIndex = 0;
            //this.Sonido();
        }
        private void Sonido()
        {
            SoundPlayer sonido = new SoundPlayer();
            sonido.SoundLocation = "D:/fondo.wav";
            sonido.Play();
        }
    }
}
