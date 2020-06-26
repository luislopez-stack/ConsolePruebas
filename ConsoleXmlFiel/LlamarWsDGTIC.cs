using ExcelDataReader;
using ICSharpCode.SharpZipLib.Zip;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleXmlFiel
{
    class LlamarWsDGTIC
    {


        //LAMADA AL WEB SERVICE//
        //public string mandarWs(int doc, int id_usuario, string sello, string preAutentXml )
        public string mandarWs(int doc, string sello, string prXml)
        {

            string errores = "";
            int bandStatus = 0;
            //int status=0;
            string err = "";
            byte[] archivoUnZip;
            //Procesos proc = new Procesos();
            statusWs zipArchive = new statusWs();
            EncodeDecode Encode = new EncodeDecode();



            //string xML = db.XMLs.Where(x => x.ID_DOCUMENTO == doc).FirstOrDefault().XML_STR;
            string xML = prXml;

            //ENCODE
            string encode = Encode.Base64Encode(xML);
            byte[] bytes = System.Convert.FromBase64String(encode);
            string dMensaje = "";
            string dNumLote = "";
            byte[] dArchivo = null;


            // Servidor de Desarrollo
            ///*
            ServiceReferencePrueba.autenticacionType autenticacion2 = new ServiceReferencePrueba.autenticacionType();
            ServiceReferencePrueba.TitulosPortTypeClient soap2 = new ServiceReferencePrueba.TitulosPortTypeClient();
            ServiceReferencePrueba.cargaTituloElectronicoRequest cRequest2 = new ServiceReferencePrueba.cargaTituloElectronicoRequest();
            ServiceReferencePrueba.cargaTituloElectronicoResponse cResponse2 = new ServiceReferencePrueba.cargaTituloElectronicoResponse();
            ServiceReferencePrueba.descargaTituloElectronicoRequest dRequest2 = new ServiceReferencePrueba.descargaTituloElectronicoRequest();
            ServiceReferencePrueba.descargaTituloElectronicoResponse dResponse2 = new ServiceReferencePrueba.descargaTituloElectronicoResponse();
            autenticacion2.usuario = "usuariomet.qa794";
            autenticacion2.password = "1egraH2QkP";
            //*/

            //CARGA DE TITULO EN SERVIDOR DE DESARROLLO//
            cRequest2.nombreArchivo = (doc + ".xml").ToString();
            cRequest2.archivoBase64 = bytes;
            cRequest2.autenticacion = autenticacion2;

            cResponse2 = soap2.cargaTituloElectronico(cRequest2);

            string cNumLote = cResponse2.numeroLote;

            //DESCARGA DE TITULO//
            dRequest2.numeroLote = cNumLote;
            dRequest2.autenticacion = autenticacion2;
            // Esperamos cinco segundos para la respuesta
            Thread.Sleep(5000);

            dResponse2 = soap2.descargaTituloElectronico(dRequest2);

            dMensaje = dResponse2.mensaje;
            dNumLote = dResponse2.numeroLote;
            dArchivo = dResponse2.titulosBase64;


            if (dArchivo == null)
            {
                string mensaje = "Error: Servicio DGP Documento: " + doc + "Lote: " + dNumLote + " Mensaje Servidor: " + dMensaje;
                string error = GrabaStObs(doc, 4, mensaje);
                errores += mensaje;
                return errores;
            }

            //UNZIP//
            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream(dArchivo))
                {
                    using (var zipInputStream = new ZipInputStream(inputStream))
                    {
                        zipInputStream.GetNextEntry();
                        zipInputStream.CopyTo(outputStream);
                    }
                    archivoUnZip = outputStream.ToArray();

                    //GUARDAR XLS
                    //File.WriteAllBytes(@"C:\Users\Desktop\ss.xls", dd);
                }
            }

            //LEER DATOS DENTRO DE XLS
            MemoryStream ms = new MemoryStream(archivoUnZip);
            Encoding codificacion = Encoding.Default;
            StreamReader sr = new StreamReader(ms, codificacion);
            string linea;
            int i = 0;
            while ((linea = sr.ReadLine()) != null)
            {
                i = i + 1;
                string lin = linea;
                if (lin.Contains("exitosamente"))
                {
                    bandStatus = 1;
                    zipArchive.status = bandStatus;
                    zipArchive.descripcion = "Título electrónico registrado exitosamente. Lote: " + dNumLote;
                }
                else
                {
                    if (lin.Contains("FOLIO"))
                    {
                        err = lin;
                        zipArchive.descripcion = Regex.Replace(err, @"[^0-9A-Za-z.:,áéíóúÁÉÍÓÚ]", " ", RegexOptions.None) + " Lote: " + dNumLote;
                        zipArchive.status = 2;
                    }
                }
            }
            sr.Close();

            //CAMBIO ESTATUS DE TITULO//
            //CAMBIAR ESTATUS
            //if (zipArchive.status == 1)
            //{
            //    string error = GrabaStObs(doc, 5, zipArchive.descripcion);

            //    if (error.Length > 4 && error.Substring(0, 5) == "Error")
            //    {
            //        errores += "Error: No se pudo cambiar ST a registro en profesiones, del documento: " + doc;
            //    }
            //    //ACTUALIZAR XML EN TABLA .. excepto Normales ... 
            //    if (preAutentXml.Length > 0)
            //    {
            //        string docXml = preAutentXml;
            //        try
            //        {
            //            XML tXML = db.XMLs.Where(x => x.ID_DOCUMENTO == doc).FirstOrDefault();
            //            if (tXML.XML_STR != docXml)
            //            {
            //                tXML.XML_STR = docXml;
            //                db.Entry(tXML).State = EntityState.Modified;
            //                db.SaveChanges();
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            //Error no se pudo actualizar Xml
            //            errores += "Error \t -(22) " + db.CAT_ERROR_MENSAJE.Find(22).DESCRIPCION + " \r\n";
            //            return (errores);
            //        }
            //    }
            //    // Guarda Sello Documento IEA
            //    db.SELLO_DOCUMENTO_IEA.Add(new SELLO_DOCUMENTO_IEA
            //    {
            //        ID_DOCUMENTO = doc,
            //        ID_TIPOSELLADO = 3,
            //        TXT_SELLO_DOCUMENTO_IEA = sello,
            //        ID_ESTATUS_SELLO = 1,
            //        FECHA_REGISTRO = DateTime.Now

            //    });
            //    try { db.SaveChanges(); }
            //    catch (Exception)
            //    {
            //        // return Conflict();
            //        errores += "Error: No se pudo actualizar Tabla SELLO_DOCUMENTO_IEA, documento: " + doc;
            //    }

            //    //BITACORA//
            //    int moov = 5;
            //    proc.GuardaBitacora(doc, (byte)moov, id_usuario);
            //}
            //else
            //{
            //    string error = GrabaStObs(doc, 4, zipArchive.descripcion);
            //    errores += "Error: " + zipArchive.descripcion;
            //    //BITACORA//
            //    int moov = 9;
            //    proc.GuardaBitacora(doc, (byte)moov, id_usuario);
            //}

            if (errores.Length == 0 && zipArchive.descripcion.Length > 0) { errores = zipArchive.descripcion; }
            return errores;
        }

        /// <summary>Funcion Cargar titulos a DGP por lotes</summary>
        /// <param name="byTitulo"></param>
        /// <param name="idLote"></param>
        /// <return name="NumeroLote por DGP"></return>
        public string cargarTitulo(byte[] byTitulo, string idLote) {

            // Servidor de Desarrollo
            ///*
            ServiceReferencePrueba.autenticacionType autenticacion2 = new ServiceReferencePrueba.autenticacionType();
            ServiceReferencePrueba.TitulosPortTypeClient soap2 = new ServiceReferencePrueba.TitulosPortTypeClient();
            ServiceReferencePrueba.cargaTituloElectronicoRequest cRequest2 = new ServiceReferencePrueba.cargaTituloElectronicoRequest();
            ServiceReferencePrueba.cargaTituloElectronicoResponse cResponse2 = new ServiceReferencePrueba.cargaTituloElectronicoResponse();
            
            autenticacion2.usuario = "usuariomet.qa794";
            autenticacion2.password = "1egraH2QkP";
            //*/

            byte[] bytes = byTitulo;

            //CARGA DE TITULO EN SERVIDOR DE DESARROLLO//
            cRequest2.nombreArchivo = (idLote+".zip").ToString();
            cRequest2.archivoBase64 = bytes;
            cRequest2.autenticacion = autenticacion2;

            cResponse2 = soap2.cargaTituloElectronico(cRequest2);

            string cNumLote = cResponse2.numeroLote;

            return cNumLote;
        }


        /// <summary>Funcion Cargar titulos a DGP por lotes</summary>
        /// <param name="numLote">toma de la tabla lotes</param>
        /// <return name="cEstusLote">Estado del Lote en DGP</return>
        public short consultaTitulo(string numLote) {

            // Servidor de Desarrollo
            ///*
            ServiceReferencePrueba.autenticacionType autenticacion2 = new ServiceReferencePrueba.autenticacionType();
            ServiceReferencePrueba.TitulosPortTypeClient soap2 = new ServiceReferencePrueba.TitulosPortTypeClient();
            ServiceReferencePrueba.consultaProcesoTituloElectronicoRequest cRequest2 = new ServiceReferencePrueba.consultaProcesoTituloElectronicoRequest();
            ServiceReferencePrueba.consultaProcesoTituloElectronicoResponse cResponse2 = new ServiceReferencePrueba.consultaProcesoTituloElectronicoResponse();

            autenticacion2.usuario = "usuariomet.qa794";
            autenticacion2.password = "1egraH2QkP";
            //*/

            // CONSULTA TITULO EN SERVIDOR DE DESARROLLO//
            cRequest2.numeroLote = numLote;
            cRequest2.autenticacion = autenticacion2;

            cResponse2 = soap2.consultaProcesoTituloElectronico(cRequest2);

            short cEstusLote = cResponse2.estatusLote;

            return cEstusLote;
        }


        public statusWsTotal descargarTitulo(string numeroLote) {

            string errores = "";
            int bandStatus = 0;
            //int status=0;
            string err = "";
            byte[] archivoUnZip;
            //Procesos proc = new Procesos();
            statusWs zipArchive = new statusWs();
            EncodeDecode Encode = new EncodeDecode();
            statusWsTotal resumen = new statusWsTotal();



            string dMensaje = "";
            string dNumLote = "";
            byte[] dArchivo = null;


            // Servidor de Desarrollo
            ///*
            ServiceReferencePrueba.autenticacionType autenticacion2 = new ServiceReferencePrueba.autenticacionType();
            ServiceReferencePrueba.TitulosPortTypeClient soap2 = new ServiceReferencePrueba.TitulosPortTypeClient();
           
            ServiceReferencePrueba.descargaTituloElectronicoRequest dRequest2 = new ServiceReferencePrueba.descargaTituloElectronicoRequest();
            ServiceReferencePrueba.descargaTituloElectronicoResponse dResponse2 = new ServiceReferencePrueba.descargaTituloElectronicoResponse();
            autenticacion2.usuario = "usuariomet.qa794";
            autenticacion2.password = "1egraH2QkP";
            //*/

           

            string cNumLote = numeroLote;

            //DESCARGA DE TITULO//
            dRequest2.numeroLote = cNumLote;
            dRequest2.autenticacion = autenticacion2;
            // Esperamos cinco segundos para la respuesta
            //Thread.Sleep(5000);

            dResponse2 = soap2.descargaTituloElectronico(dRequest2);

            dMensaje = dResponse2.mensaje;
            dNumLote = dResponse2.numeroLote;
            dArchivo = dResponse2.titulosBase64;


            if (dArchivo == null)
            {
                string mensaje = "Error: Servicio DGP Documento: "; //+ doc + "Lote: " + dNumLote + " Mensaje Servidor: " + dMensaje;
                //string error = GrabaStObs(doc, 4, mensaje);
                resumen.errores += mensaje;
                return resumen;
            }

            //UNZIP//
            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream(dArchivo))
                {
                    using (var zipInputStream = new ZipInputStream(inputStream))
                    {
                        zipInputStream.GetNextEntry();
                        zipInputStream.CopyTo(outputStream);
                    }
                    archivoUnZip = outputStream.ToArray();

                    //GUARDAR XLS
                    File.WriteAllBytes(@"C:\Users\Luis\Documents\"+dNumLote+".xls", archivoUnZip);
                }
            }


            //NUEVO LEER BYTES
            List<CamposXls> resunemXls = leerArchivoRespuesta(dNumLote);
            byte status = 0;
            string docxml;
            resumen.NumLote = dNumLote;
            foreach (CamposXls resp in resunemXls) {

                resumen.TotalTitulos += 1;

                switch (resp.Estatus) {

                    case "1" :
                        resumen.Correctos += 1;
                        status = 5;
                        docxml = Regex.Replace(resp.Archivo, @"[^0-9]","", RegexOptions.None);
                        break;

                    case "2":
                        resumen.Fallidos += 1;
                        status = 4;
                        docxml = Regex.Replace(resp.Archivo, @"[^0-9]", "", RegexOptions.None);
                        resumen.descripcion = resp.Descripcion;
                        break;

                    case " ":
                        resumen.SinRespuesta += 1;
                        status = 4; 
                        break;

                }

                //string error = GrabaStObs(doc, status, resp.Descripcion);
                //    if (error.Length > 4 && error.Substring(0, 5) == "Error")
                //    {
                //        errores += "Error: No se pudo cambiar ST a registro en profesiones, del documento: " + doc;
                //    }

            }

            //File.Delete(@"C:\Users\Luis\Documents\" + dNumLote + ".xls");

            

            
            return resumen;

        }



        public string GrabaStObs(int doc, byte st, string obs)
        {
            // Procesos proc = new Procesos();
            string error = "";
            try
            {
                //DOC_TITULO_PROFESION dOC_TITULO_PROFESION = db.DOC_TITULO_PROFESION.Find(doc);
                //dOC_TITULO_PROFESION.ID_ESTATUS = st; //Rechazado
                //dOC_TITULO_PROFESION.OBSERVACION = obs;
                //db.Entry(dOC_TITULO_PROFESION).State = EntityState.Modified;
                //db.SaveChanges();

            }
            catch (Exception)
            {
                error = "Error: no se pudo actualizar ST y/o Observación ";
                /*Error no se pudo actualizar Xml*/
            }
            //BITACORA//
            // int moov = 9;
            // proc.GuardaBitacora(doc, (byte)moov, id_usuario);
            return error;
        }






        public List<CamposXls> leerArchivoRespuesta(string dNumLote) {

            //LEER DATOS DENTRO DE XLS
            //string xls = @"C:\Users\Luis\Documents\"+ dNumLote+".xls";
            string xls = @"C:\Users\Luis\Documents\"+ dNumLote+".xls";
            //ExcelQueryFactory book = new ExcelQueryFactory(dArchivo);

            var book = new ExcelQueryFactory(xls);
            var resultado = (from row in book.Worksheet("Resultado")
                             let item = new CamposXls
                             {
                                 Archivo = row["ARCHIVO"],
                                 Estatus = row["ESTATUS"],
                                 Descripcion = row["DESCRIPCION"]
                                 //Folio = row["FOLIOCONTROL"]
                             }
                             select item).ToList();

            book.Dispose();
            return resultado;
        }
    }
}
