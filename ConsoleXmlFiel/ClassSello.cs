using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleXmlFiel
{
    class ClassSello
    {
        public string CrearSello(string cadenaOr)
        {
            string sellodigital = "";
            try
            {
                //string archivoCertificado = "";
                string key = @"C:\Users\Luis\Documents\MiTitulo\FIEL_AULJ951012PZ7_20190328160816\Claveprivada_FIEL_AULJ951012PZ7_20190328_160816.key";
                string escadiakey = @"C:\Users\Luis\Documents\MiTitulo\FIEL_AULJ951012PZ7_20190328160816\Claveprivada_FIEL_VACS630322M81_20180201_130231.key";
                string prueba1key = @"C:\Users\Luis\Documents\MiTitulo\FIEL_AULJ951012PZ7_20190328160816\CSD02_AAA010101AAA.key";
                string prueba2key = @"C:\Users\Luis\Documents\MiTitulo\FIEL_AULJ951012PZ7_20190328160816\CSD03_AAA010101AAA.key";
                string cedulaskey = @"C:\Users\Luis\Documents\MiTitulo\FIEL_AULJ951012PZ7_20190328160816\Claveprivada_FIEL_SOLR761006UN5_20191112_125540.key";

                string lPassword = "12Descargaspass";
                string escadiaPass = "Dmgo1604";
                string pruebaPass = "12345678a";
                string cedulasPass = "lacasademistias";
                string strCadenaOriginal = cadenaOr;
                SecureString identidad = new SecureString();// Se requerira un objeto SecureString que represente el password de la clave privada, que se obtiene asi:
                identidad.Clear();
                foreach (char c in lPassword.ToCharArray())
                //foreach (char c in cedulasPass.ToCharArray())
                {
                    identidad.AppendChar(c);
                }

                Byte[] llavePrivada = System.IO.File.ReadAllBytes(key);
                //Byte[] llavePrivada = System.IO.File.ReadAllBytes(cedulaskey);
                RSACryptoServiceProvider rsa = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivada, identidad);// Uso de la clase opensslkey
                SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
                Byte[] bytesFirmados = rsa.SignData(System.Text.Encoding.UTF8.GetBytes(strCadenaOriginal), hasher);
                sellodigital = Convert.ToBase64String(bytesFirmados);// Obtengo Sello

            }
            catch (Exception ex) { Console.WriteLine("Error al sellar" + ex); }

            return sellodigital;
        }


    }
}
