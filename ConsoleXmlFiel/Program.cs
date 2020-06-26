using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


//using Ionic.Zip;


namespace ConsoleXmlFiel
{
    class Program
    {

        static void Main(string[] args)
        {
            //string zip = @"C:\Users\Luis\Documents\Arduino\229747.zip";
            //Byte[] unzip = System.IO.File.ReadAllBytes(zip);
            //string res = leerexcell(unzip);
            //Console.WriteLine("Respuesta DGTIC:" + res);
            //Console.ReadLine();
            
            LlamarWsDGTIC wsDGTIC = new LlamarWsDGTIC();
            statusWsTotal wsTotal = new statusWsTotal();
            statusWsTotal resp;
            statusWsTotal count;
            EncodeDecode Encode = new EncodeDecode();
            //wsDGTIC.leerexcell(deco);
            //CamposXls lis = wsDGTIC.leerArchivoRespuesta(decode);
            //foreach (var elemento in lis)
            //{
            //    Console.WriteLine("hola " + elemento);
            //}


            //CREAR CADENA ORIGINAL
            cadenaOrig cadenaOrig = new cadenaOrig();
            string cdo = cadenaOrig.CadenaOriginal();

            //SELLAR
            ClassSello classSello = new ClassSello();
            string selloxml = classSello.CrearSello(cdo);
            Console.WriteLine("Sello con cadena original: " + selloxml);
            Console.ReadLine();
            string pipe = ArmaPipes2(selloxml);
            Console.WriteLine("Cadena original autenticacion:" + pipe);
            Console.ReadLine();



            //MANDAR WS
            string xmlStr1 = "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>  <TituloElectronico xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" version=\"1.0\" folioControl=\"UCA147001123\" xsi:schemaLocation=\"https://www.siged.sep.gob.mx/titulos/schema.xsd\" xmlns=\"https://www.siged.sep.gob.mx/titulos/\" >    <FirmaResponsables>      <FirmaResponsable nombre=\"ROGELIO\" primerApellido=\"MARTINEZ\" segundoApellido=\"BRIONES\" curp=\"MABR441101HJCRRG07\" idCargo=\"3\" cargo=\"RECTOR\" abrTitulo=\"Lic.\" sello=\"Q1rGQ5rMWXETxPjcKwFCIPVtgknydw5WRlVud7iyG1lVhVawW6cOxDBiPsOi5UtjG6Nz36B /mXyNA60vaVBcD9/Komd/Vqasyefi6FMjWVsf9MOY4hs5j67mz+v4TgRPWJ/FJFkqQTH5mhlwin6eSZ1TRmBB9Z0ohv2K0+IZNgw8WBp6RndLuYmgM1eu4pONUL/m8gkp/+ylQXpLJlw41eBafazoV0pSJycuqIUffhyMELpeOZMMGttSWskZdbH6BfFvWgu1hW5P7hz5iCvoCirmOl6FBxpv5411vPpL2245CubH5RZV/8gM3RWdHA5aBhiKuuqeboRAkF+FW8kPsg==\" certificadoResponsable =\"MIIGZTCCBE2gAwIBAgIUMDAwMDEwMDAwMDA0MDIwNjY4NzYwDQYJKoZIhvcNAQELBQAwggGyMTgwNgYDVQQDDC9BLkMuIGRlbCBTZXJ2aWNpbyBkZSBBZG1pbmlzdHJhY2nDs24gVHJpYnV0YXJpYTEvMC0GA1UECgwmU2VydmljaW8gZGUgQWRtaW5pc3RyYWNpw7NuIFRyaWJ1dGFyaWExODA2BgNVBAsML0FkbWluaXN0cmFjacOzbiBkZSBTZWd1cmlkYWQgZGUgbGEgSW5mb3JtYWNpw7NuMR8wHQYJKoZIhvcNAQkBFhBhY29kc0BzYXQuZ29iLm14MSYwJAYDVQQJDB1Bdi4gSGlkYWxnbyA3NywgQ29sLiBHdWVycmVybzEOMAwGA1UEEQwFMDYzMDAxCzAJBgNVBAYTAk1YMRkwFwYDVQQIDBBEaXN0cml0byBGZWRlcmFsMRQwEgYDVQQHDAtDdWF1aHTDqW1vYzEVMBMGA1UELRMMU0FUOTcwNzAxTk4zMV0wWwYJKoZIhvcNAQkCDE5SZXNwb25zYWJsZTogQWRtaW5pc3RyYWNpw7NuIENlbnRyYWwgZGUgU2VydmljaW9zIFRyaWJ1dGFyaW9zIGFsIENvbnRyaWJ1eWVudGUwHhcNMTYwNDE0MTk1ODQwWhcNMjAwNDE0MTk1OTIwWjCB0zEhMB8GA1UEAxMYUk9HRUxJTyBNQVJUSU5FWiBCUklPTkVTMSEwHwYDVQQpExhST0dFTElPIE1BUlRJTkVaIEJSSU9ORVMxITAfBgNVBAoTGFJPR0VMSU8gTUFSVElORVogQlJJT05FUzELMAkGA1UEBhMCTVgxJjAkBgkqhkiG9w0BCQEWF2JyaXpha2FyaW5hQGhvdG1haWwuY29tMRYwFAYDVQQtEw1NQUJSNDQxMTAxNTMwMRswGQYDVQQFExJNQUJSNDQxMTAxSEpDUlJHMDcwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCTkFalGFETZxrBSjUrY5suq86 /UM1MhP27sHWtZ6kWIkxRKp7AnQUtVW/KGG1dwBSlLnqJ+GmDJVml1E1mipR6xyaggnFWEuHky3xpZ0qbBgRJ6RS5pZRH9dGJWipL/2UodRK2tyYxeS+MqebEhf+IuuwLyxHTF65dvQDk6cJ5HSIwInoGk5sQXhfL6qZs2ZXb4MNv7XvkugRCkmeJV66Gq6iy4j3PZUBl4GGr0QCfLcT0psX9fCDzNDWwNIh+MIvQz00HUxvEEJlxi8hq7akgOFt71Bl3HFYK8peTIHblTGtQIvZFFg329d7zcOP1L/wAKFTNrTAn897QsWFm0ATFAgMBAAGjTzBNMAwGA1UdEwEB/wQCMAAwCwYDVR0PBAQDAgPYMBEGCWCGSAGG+EIBAQQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDBAYIKwYBBQUHAwIwDQYJKoZIhvcNAQELBQADggIBAK5QpvLT5aJEjalSV02z4ugnjZVIeWSdT7mCiRuErt4q7MD9daMS8DEl1F2h+9TrkJbnftR+AZCeCD10tLz8ogpEAeMXSOxh83fauKHEhH/D0xUp/SRzIPthbDijhVBGR7eTa/jRxp0Pc86mIi3tPIjtXcS9v2UwEX7kJZ7ppepM0XLgZ7bvSSmSCBbiyoOMXapLz7M04xhdwtvTmt69CwGyo+Y/Q7kWeNy+7Kif16gbCYUHU7n2YzuXgHMIYxR2SoD3SzT/Bprzt7Wf81Ijl10dBi9XC1OYzPVBUnyyW9fT7eHGuCKrVCnpYC4OjYfuhKTp4wZfhfOTErX3fOo9RybdCX55RYJ95KPwshQJ1xyhs+XR1geZkhxu++Y75pzF26zG4FDMvkey7KzAcm+lhoNdzMo4H8NU/tcC1vmeADUoFLkRa8XNAUUUIjva4uKzyT6AgL0UZWoy6DtBmY1sLAJlgWy58HCERnHwyv+YkeXvQdMR4XXz0xM8RB6PbJ8rKOYYNUFFvIX2YeHG8iMUC7Ppdzwq9Lmrxqp5P2VT4Xy2luGY7fjYl5qB2z4FIPnRCdPwZe4hOzEjjRVc+jWXutO4MSjXZbKQcWUs2FT/lB7TOIaQHZbId5tCeuqf6+0sGfHVrk81MtUzkrqszyNxyEv5g50Je5FlQ/hc97Uaq5I2\" noCertificadoResponsable=\"00001000000402066876\" />    </FirmaResponsables>    <Institucion cveInstitucion=\"010045\" nombreInstitucion=\"UNIVERSIDAD CUAUHTÉMOC PLANTEL AGUASCALIENTES\" />    <Carrera cveCarrera=\"612301\" nombreCarrera=\"LICENCIATURA EN DERECHO\" fechaInicio=\"2017-01-09\" fechaTerminacion=\"2019-01-14\" idAutorizacionReconocimiento=\"2\" autorizacionReconocimiento=\"RVOE ESTATAL\" numeroRvoe=\"1470\" />    <Profesionista curp=\"SEGI960901MASRRR06\" nombre=\"IRIS NATALI\" primerApellido=\"SERRANO\" segundoApellido=\"GARZA\" correoElectronico=\"control.escolar@ucuauhtemoc.edu.mx\" />    <Expedicion fechaExpedicion=\"2019-05-10\" idModalidadTitulacion=\"6\" modalidadTitulacion=\"OTRO\" fechaExencionExamenProfesional=\"2019-04-05\" cumplioServicioSocial=\"1\" idFundamentoLegalServicioSocial=\"4\" fundamentoLegalServicioSocial=\"ART. 10 REGLAMENTO PARA LA PRESTACIÓN DEL SERVICIO SOCIAL DE LOS ESTUDIANTES DE LAS INSTITUCIONES DE EDUCACIÓN SUPERIOR EN LA REPÚBLICA MEXICANA\" idEntidadFederativa=\"01\" entidadFederativa=\"AGUASCALIENTES\" />    <Antecedente institucionProcedencia=\"COLEGIO DE ESTUDIOS CIENTÍFICOS Y TECNOLÓGICOS DEL ESTADO DE AGUASCALIENTES\" idTipoEstudioAntecedente=\"4\" tipoEstudioAntecedente=\"BACHILLERATO\" idEntidadFederativa=\"01\" entidadFederativa=\"AGUASCALIENTES\" fechaInicio=\"2011-08-22\" fechaTerminacion=\"2014-07-15\" />  </TituloElectronico>";
            int idDocu = 810;
            string xmlStr2 = "﻿<?xml version=\"1.0\" encoding =\"utf - 8\"?>  <TituloElectronico xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" version =\"1.0\" folioControl =\"31323334\" xsi:schemaLocation=\"https://www.siged.sep.gob.mx/titulos/schema.xsd\" xmlns =\"https://www.siged.sep.gob.mx/titulos/\" >    <FirmaResponsables>      <FirmaResponsable nombre=\"JUAN LUIS\" primerApellido =\"AGUILAR\" segundoApellido =\"LOPEZ\" curp =\"AULJ951012HASGPN08\" idCargo =\"3\" cargo =\"RECTOR\" abrTitulo =\"LIC.\" sello =\"R0mytHMvl2SGc2nv0hJ /ucSGyE0KYzSvXQYiB8ueWVlG2mkSEYdNOsJSLDgrClayvkPRHDj7dEHN0yU8BoHapa7dBxmYpSjjz38mwzQOM1Prcb7V1dpL8v8tKPW6/yqNco8NaUMimLmO3l+aVTqYnh7K54KKe3F2Q3Cv82VisStVQ5baDuHKz3hEEDkODePh921Q7Be/0ieEvX/b64r/XtZfsmQdncqyfiNoHLGCrcfCa0MSzwVMYHpDKhV56Az0IIW00frRuUPRLt2MpMrvg1xTLm52K6vgyyDSrB09hF0WKnvbqFGWeAAyuv/9JieQjeM6Gmz16KpsmwrGKn/RpQ==\" certificadoResponsable =\"MIIGaTCCBFGgAwIBAgIUMDAwMDEwMDAwMDA0MTQwOTMxMDkwDQYJKoZIhvcNAQELBQAwggGyMTgwNgYDVQQDDC9BLkMuIGRlbCBTZXJ2aWNpbyBkZSBBZG1pbmlzdHJhY2nDs24gVHJpYnV0YXJpYTEvMC0GA1UECgwmU2VydmljaW8gZGUgQWRtaW5pc3RyYWNpw7NuIFRyaWJ1dGFyaWExODA2BgNVBAsML0FkbWluaXN0cmFjacOzbiBkZSBTZWd1cmlkYWQgZGUgbGEgSW5mb3JtYWNpw7NuMR8wHQYJKoZIhvcNAQkBFhBhY29kc0BzYXQuZ29iLm14MSYwJAYDVQQJDB1Bdi4gSGlkYWxnbyA3NywgQ29sLiBHdWVycmVybzEOMAwGA1UEEQwFMDYzMDAxCzAJBgNVBAYTAk1YMRkwFwYDVQQIDBBEaXN0cml0byBGZWRlcmFsMRQwEgYDVQQHDAtDdWF1aHTDqW1vYzEVMBMGA1UELRMMU0FUOTcwNzAxTk4zMV0wWwYJKoZIhvcNAQkCDE5SZXNwb25zYWJsZTogQWRtaW5pc3RyYWNpw7NuIENlbnRyYWwgZGUgU2VydmljaW9zIFRyaWJ1dGFyaW9zIGFsIENvbnRyaWJ1eWVudGUwHhcNMTkwMzI4MjIyMzUyWhcNMjMwMzI4MjIyNDMyWjCB1zEgMB4GA1UEAxMXSlVBTiBMVUlTIEFHVUlMQVIgTE9QRVoxIDAeBgNVBCkTF0pVQU4gTFVJUyBBR1VJTEFSIExPUEVaMSAwHgYDVQQKExdKVUFOIExVSVMgQUdVSUxBUiBMT1BFWjELMAkGA1UEBhMCTVgxLTArBgkqhkiG9w0BCQEWHmx1aXMuYWd1aWxhci5sb3BlejE3QGdtYWlsLmNvbTEWMBQGA1UELRMNQVVMSjk1MTAxMlBaNzEbMBkGA1UEBRMSQVVMSjk1MTAxMkhBU0dQTjA4MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjRrSCVboAJMSowApd8mH3sTrhegucKaZ7pdJyZiFkwYUzF0F +PLOCQbIp9ZsaKBK7VM5YBCoP8RR6pxW6JTVPUHJDQx2CIPERxW5N4yFQ2wXypZJQAjq6uxOVm+9mi703FD9oaBzTH6a0PBvhorolOF4skNuHxyJDIrP9Lx83PnpPHZtHBibZoErdZ8p3r8weBN3h7alR2cJtOV+DRxvByeHrEPK9tVfZNCnaDJXzC3OBOKWzFZzpDqZvlXI4eRKpz9jdlL8q/xAx90ObVoqRlXwUivv/InTgQXO3jGQnxpMGoUNG5120u0hp6Ib7AvHPyNUHVnPu8+ThtljqpYgHwIDAQABo08wTTAMBgNVHRMBAf8EAjAAMAsGA1UdDwQEAwID2DARBglghkgBhvhCAQEEBAMCBaAwHQYDVR0lBBYwFAYIKwYBBQUHAwQGCCsGAQUFBwMCMA0GCSqGSIb3DQEBCwUAA4ICAQAlBcA2UipLgayo4jYqzgjqHiWAjNbZdTDkxLPpYyFrM7SeHs17aXYfwMwQsKcve2Rk5DxhSiXof/licn+Bhp5sb2XC3H+9plAknhJWO66B5CyGoiYxeqIMDDFeMlUmTJr0Y9WY7R+Mw3DRQp7GI4EIcYBRBOW9Gv8kk+tvI+BE39xYU4GeOhopqKriUXa4Y5BmIJ+j/UrcRSM7jGJ3lc/ki5y4SvXI8D9mA47IEMPlWwcR5ZYkMw15eb+dVb30LwxYDsGjvo0KnrWqNucTKr02ns+gtSoq/dK01pjUNdSJpCQzVgIHEnrDZNZA2dv917vwMOE4KaqxSTTR6rol+rWtlau+BDqnBPgs/kIjMFQBgWwEBH+rUPI6p5YzzLK8D6DL5apfRt6CO4vPR5MPoIOneIf8V7Tfbh4HJp2n+t7jp1Cncq8bUtq4YyLDU/RBrD8NydZy/PEtXkSaMLqmFQ3IICQtEC9Z9I97D2mES6vFw07adV2avnak5MfRcnxk9fL9/dGqD2Flj07GLoH8ePjOM7vuNOVgMrReavy3s2XFFpyVmxDRokpbPDyftEtH904dbUfiqddaF0ui8XSZRrmAdv5jVB+TM+SC6LPNtWA0MEKRpIvMzwCzpR614Yl3kz74o07Oh2+o/8Z32LvKVCrxmiTdI4VYSRNFU/fDtxS/jw==\" noCertificadoResponsable =\"00001000000414093109\" />    </FirmaResponsables>    <Institucion cveInstitucion=\"010045\" nombreInstitucion =\"UNIVERSIDAD CUAUHTÉMOC PLANTEL AGUASCALIENTES\" />    <Carrera cveCarrera=\"502301\" nombreCarrera =\"LICENCIATURA EN ARQUITECTURA DE INTERIORES\" fechaInicio =\"2015 -09-01\" fechaTerminacion =\"2019 -01-09\" idAutorizacionReconocimiento =\"4\" autorizacionReconocimiento =\"AUTORIZACIÓN ESTATAL\" numeroRvoe =\"2159\" />    <Profesionista curp=\"MARA860226MASRDN09\" nombre =\"ANA LILINA\" primerApellido =\"MARTINEZ\" segundoApellido =\"RODRIGUEZ\" correoElectronico =\"ap @gmail.com\" />    <Expedicion fechaExpedicion=\"2020 -06-16\" idModalidadTitulacion =\"2\" modalidadTitulacion =\"POR PROMEDIO\" fechaExencionExamenProfesional =\"2020 -06-01\" cumplioServicioSocial =\"1\" idFundamentoLegalServicioSocial =\"2\" fundamentoLegalServicioSocial =\"ART. 55 LRART. 5 CONST\" idEntidadFederativa =\"01\" entidadFederativa =\"AGUASCALIENTES\" />    <Antecedente institucionProcedencia=\"BACHILLERATO DE LA CIUDAD\" idTipoEstudioAntecedente =\"4\" tipoEstudioAntecedente =\"BACHILLERATO\" idEntidadFederativa =\"15\" entidadFederativa =\"MÉXICO\" fechaInicio =\"2012 -08-10\" fechaTerminacion =\"2014 -01-31\" noCedula =\"\" />  </TituloElectronico>";

            string[] xmlStr = {
                "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>  <TituloElectronico xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" version=\"1.0\" folioControl=\"31323334\" xsi:schemaLocation=\"https://www.siged.sep.gob.mx/titulos/schema.xsd\" xmlns=\"https://www.siged.sep.gob.mx/titulos/\" >    <FirmaResponsables>      <FirmaResponsable nombre=\"JUAN LUIS\" primerApellido=\"AGUILAR\" segundoApellido=\"LOPEZ\" curp=\"AULJ951012HASGPN08\" idCargo=\"3\" cargo=\"RECTOR\" abrTitulo=\"LIC.\" sello=\"R0mytHMvl2SGc2nv0hJ /ucSGyE0KYzSvXQYiB8ueWVlG2mkSEYdNOsJSLDgrClayvkPRHDj7dEHN0yU8BoHapa7dBxmYpSjjz38mwzQOM1Prcb7V1dpL8v8tKPW6/yqNco8NaUMimLmO3l+aVTqYnh7K54KKe3F2Q3Cv82VisStVQ5baDuHKz3hEEDkODePh921Q7Be/0ieEvX/b64r/XtZfsmQdncqyfiNoHLGCrcfCa0MSzwVMYHpDKhV56Az0IIW00frRuUPRLt2MpMrvg1xTLm52K6vgyyDSrB09hF0WKnvbqFGWeAAyuv/9JieQjeM6Gmz16KpsmwrGKn/RpQ==\" certificadoResponsable =\"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\" noCertificadoResponsable=\"00001000000414093109\" />    </FirmaResponsables>    <Institucion cveInstitucion=\"010045\" nombreInstitucion=\"UNIVERSIDAD CUAUHTÉMOC PLANTEL AGUASCALIENTES\" />    <Carrera cveCarrera=\"502301\" nombreCarrera=\"LICENCIATURA EN ARQUITECTURA DE INTERIORES\" fechaInicio=\"2015-09-01\" fechaTerminacion=\"2019-01-09\" idAutorizacionReconocimiento=\"4\" autorizacionReconocimiento=\"AUTORIZACIÓN ESTATAL\" numeroRvoe=\"2159\" />    <Profesionista curp=\"MARA860226MASRDN09\" nombre=\"ANA LILINA\" primerApellido=\"MARTINEZ\" segundoApellido=\"RODRIGUEZ\" correoElectronico=\"ap @gmail.com\" />    <Expedicion fechaExpedicion=\"2020-06-16\" idModalidadTitulacion=\"2\" modalidadTitulacion=\"POR PROMEDIO\" fechaExencionExamenProfesional=\"2020-06-01\" cumplioServicioSocial=\"1\" idFundamentoLegalServicioSocial=\"2\" fundamentoLegalServicioSocial=\"ART. 55 LRART. 5 CONST\" idEntidadFederativa=\"01\" entidadFederativa=\"AGUASCALIENTES\" />    <Antecedente institucionProcedencia=\"BACHILLERATO DE LA CIUDAD\" idTipoEstudioAntecedente=\"4\" tipoEstudioAntecedente=\"BACHILLERATO\" idEntidadFederativa=\"15\" entidadFederativa=\"MÉXICO\" fechaInicio=\"2012-08-10\" fechaTerminacion=\"2014-01-31\" noCedula=\"\" />  </TituloElectronico>",
                "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>  <TituloElectronico xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" version=\"1.0\" folioControl=\"UCA147001123\" xsi:schemaLocation=\"https://www.siged.sep.gob.mx/titulos/schema.xsd\" xmlns=\"https://www.siged.sep.gob.mx/titulos/\" >    <FirmaResponsables>      <FirmaResponsable nombre=\"ROGELIO\" primerApellido=\"MARTINEZ\" segundoApellido=\"BRIONES\" curp=\"MABR441101HJCRRG07\" idCargo=\"3\" cargo=\"RECTOR\" abrTitulo=\"Lic.\" sello=\"Q1rGQ5rMWXETxPjcKwFCIPVtgknydw5WRlVud7iyG1lVhVawW6cOxDBiPsOi5UtjG6Nz36B /mXyNA60vaVBcD9/Komd/Vqasyefi6FMjWVsf9MOY4hs5j67mz+v4TgRPWJ/FJFkqQTH5mhlwin6eSZ1TRmBB9Z0ohv2K0+IZNgw8WBp6RndLuYmgM1eu4pONUL/m8gkp/+ylQXpLJlw41eBafazoV0pSJycuqIUffhyMELpeOZMMGttSWskZdbH6BfFvWgu1hW5P7hz5iCvoCirmOl6FBxpv5411vPpL2245CubH5RZV/8gM3RWdHA5aBhiKuuqeboRAkF+FW8kPsg==\" certificadoResponsable =\"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\" noCertificadoResponsable=\"00001000000402066876\" />    </FirmaResponsables>    <Institucion cveInstitucion=\"010045\" nombreInstitucion=\"UNIVERSIDAD CUAUHTÉMOC PLANTEL AGUASCALIENTES\" />    <Carrera cveCarrera=\"612301\" nombreCarrera=\"LICENCIATURA EN DERECHO\" fechaInicio=\"2017-01-09\" fechaTerminacion=\"2019-01-14\" idAutorizacionReconocimiento=\"2\" autorizacionReconocimiento=\"RVOE ESTATAL\" numeroRvoe=\"1470\" />    <Profesionista curp=\"SEGI960901MASRRR06\" nombre=\"IRIS NATALI\" primerApellido=\"SERRANO\" segundoApellido=\"GARZA\" correoElectronico=\"control.escolar@ucuauhtemoc.edu.mx\" />    <Expedicion fechaExpedicion=\"2019-05-10\" idModalidadTitulacion=\"6\" modalidadTitulacion=\"OTRO\" fechaExencionExamenProfesional=\"2019-04-05\" cumplioServicioSocial=\"1\" idFundamentoLegalServicioSocial=\"4\" fundamentoLegalServicioSocial=\"ART. 10 REGLAMENTO PARA LA PRESTACIÓN DEL SERVICIO SOCIAL DE LOS ESTUDIANTES DE LAS INSTITUCIONES DE EDUCACIÓN SUPERIOR EN LA REPÚBLICA MEXICANA\" idEntidadFederativa=\"01\" entidadFederativa=\"AGUASCALIENTES\" />    <Antecedente institucionProcedencia=\"COLEGIO DE ESTUDIOS CIENTÍFICOS Y TECNOLÓGICOS DEL ESTADO DE AGUASCALIENTES\" idTipoEstudioAntecedente=\"4\" tipoEstudioAntecedente=\"BACHILLERATO\" idEntidadFederativa=\"01\" entidadFederativa=\"AGUASCALIENTES\" fechaInicio=\"2011-08-22\" fechaTerminacion=\"2014-07-15\" />  </TituloElectronico>"  
            };
            int[] idDoc = { 810, 811};


            //string enc = Encode.Base64Encode(xmlStr1);
            //Console.WriteLine("Encode xml String: {0}", enc);

            //RECIVIR UNA CADENA DE IDS_DOCUMENTOS
            int totalDocMandar = 2;

            //CrearDirectorio();

            for (int i = 0; i < totalDocMandar; i++) {

                //se extrae el string de la tabala XML campo xml_str
                //para crear un archivo por cada uni de ellos
                CrearDocumento(idDoc[i], xmlStr[i]);

            }


            CompressZip compressZip = new CompressZip();
            byte[] byTitulo = compressZip.compressDirectory(@"C:\Users\Luis\Documents\LoteAEnviarDgp", @"C:\Users\Luis\Documents\LoteAEnviarDgp\loteZip.zip", 9);

            //ENCODE BASE64 ZIP
            string bs64enc = Encode.Base64Encode(byTitulo);
            Console.WriteLine("Encode BYTE1 String: {0}", bs64enc);

            byte[] base64EncodedStringBytes = Encoding.ASCII.GetBytes(Convert.ToBase64String(byTitulo));
            Console.WriteLine("Encode BYTE2 String: {0}", base64EncodedStringBytes);

            byte[] newByteArray = Encoding.UTF8.GetBytes(Convert.ToBase64String(byTitulo));
            Console.WriteLine("Encode BYTE3 String: {0}", newByteArray);



            //byte[] readby = File.ReadAllBytes(@"C:\Users\Luis\Documents\LoteAEnviarDgp\loteZip.zip");




            //GET BYTES DEL ENCODE
            byte[] gtbts = System.Convert.FromBase64String(bs64enc);
            //byte[] gtbts = Encode.GetBytes(bs64enc);

            

            //doc = 15454;
            //string sello = selloxml;
            string prXml = "";
            string idLoteZip = "1112"; 
            string cargaResp = wsDGTIC.cargarTitulo(gtbts, idLoteZip);
            Console.WriteLine("Respuesta WsDesarrollo Carga: {0}", cargaResp);
            Console.ReadLine();



            //CONSULTAWS
            //Este es un proceso ASYC 
            //toma todos los lotes con estatus 0 de la tabla de lotes y pregunta por ellos en dgp
            short estlote = wsDGTIC.consultaTitulo(cargaResp);

            //Si el estatus del lote es 1 procede a descargarlo
            if (estlote==1)
                Console.WriteLine("Lote en estado: {0}. Listo para procesar ", estlote);
            Console.ReadLine();



            //DESCARGAR WS
            //int doc = 15001;
            string numeroLote = cargaResp;
            resp = wsDGTIC.descargarTitulo(numeroLote);
            Console.WriteLine("Respuesta WsDesarrollo Lote: {0}", resp.NumLote);
            Console.WriteLine("Respuesta WsDesarrollo Totales: {0}", resp.TotalTitulos);
            Console.WriteLine("Respuesta WsDesarrollo Correctos: {0}", resp.Correctos);
            Console.WriteLine("Respuesta WsDesarrollo Fallidos: {0}", resp.Fallidos);
            Console.WriteLine("Respuesta WsDesarrollo Descripcion: {0}", resp.descripcion);
            Console.WriteLine("Respuesta WsDesarrollo Sin respuesta: {0}", resp.SinRespuesta);
            Console.WriteLine("Respuesta WsDesarrollo Errores: {0}", resp.errores);

            Console.ReadLine();


            //ENCODE

            string encode = Encode.Base64Encode(xmlStr1);
            byte[] bytes = System.Convert.FromBase64String(encode);

            Console.WriteLine("Encode: {0} ", encode);
            Console.WriteLine("Byte: {0} ", bytes);

            //string pipes = armarPipes();
            //Console.WriteLine("Armarpipes cadena original:" + pipes);
            Console.ReadLine();

            



            



            //void CrearXml(){
            XNamespace defnamespace = "https://www.siged.sep.gob.mx/titulos/";
            XNamespace xsinamespace = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace shnamespace = "https://www.siged.sep.gob.mx/titulos/schema.xsd";

            XDocument docXml = new XDocument(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement(defnamespace + "TituloElectronico",
                            new XAttribute(XNamespace.Xmlns + "xsi", xsinamespace),
                            new XAttribute("version", "1.0"),
                            new XAttribute("folioControl", "20121023SICERT0151610X0042086X00013"),
                            new XAttribute(xsinamespace + "schemaLocation", shnamespace),


                                new XElement(defnamespace + "FirmaResponsables",
                                    new XElement(defnamespace + "FirmaResponsable",
                                        new XAttribute("nombre", ""),
                                        new XAttribute("primerApellido", ""),
                                        new XAttribute("segundoApellido", ""),
                                        new XAttribute("curp", ""),
                                        new XAttribute("idCargo", ""),
                                        new XAttribute("cargo", ""),
                                        new XAttribute("abrTitulo", ""),
                                        new XAttribute("sello", "LUIS"),
                                        new XAttribute("certificadoResponsable", ""),
                                        new XAttribute("noCertificadoResponsable", "")
                                    ),
                                    new XElement(defnamespace + "FirmaResponsable",
                                        new XAttribute("nombre", ""),
                                        new XAttribute("primerApellido", ""),
                                        new XAttribute("segundoApellido", ""),
                                        new XAttribute("curp", ""),
                                        new XAttribute("idCargo", ""),
                                        new XAttribute("cargo", ""),
                                        new XAttribute("abrTitulo", ""),
                                        new XAttribute("sello", ""),
                                        new XAttribute("certificadoResponsable", ""),
                                        new XAttribute("noCertificadoResponsable", "")
                                    )
                                ),

                                new XElement(defnamespace + "Institucion",
                                    new XAttribute("cveInstitucion", "070047"),
                                    new XAttribute("nombreInstitucion", "INSTITUTO POLITÉCNICO NACIONAL")
                                ),

                                new XElement(defnamespace + "Carrera",
                                    new XAttribute("cveCarrera", ""),
                                    new XAttribute("nombreCarrera", ""),
                                    new XAttribute("fechaInicio", ""),
                                    new XAttribute("fechaTerminacion", ""),
                                    new XAttribute("idAutorizacionReconocimiento", ""),
                                    new XAttribute("AutorizacionReconocimiento", ""),
                                    new XAttribute("numeroRvoe", "") ///QUITAR
                                ),

                                new XElement(defnamespace + "Profesionista",
                                     new XAttribute("curp", "AOCJ000205MDFRNL01"),
                                     new XAttribute("nombre", "JAZMIN"),
                                     new XAttribute("primerApellido", "ACOSTA"),
                                     new XAttribute("segundoApellido", "CABRERA"),
                                     new XAttribute("correoElectronico", "jazmin.acosta@gmail.com")
                                ),

                                new XElement(defnamespace + "Expedicion",
                                    new XAttribute("fechaExpedicion", "2018 -02-13"),
                                    new XAttribute("idModalidadTitulacion", "1"),
                                    new XAttribute("modalidadTitulacion", "POR TESIS"),
                                    new XAttribute("fechaExamenProfesional", "2017 -12-13"),
                                    new XAttribute("fechaExencionExamenProfesional", ""),
                                    new XAttribute("cumplioServicioSocial", "1"),
                                    new XAttribute("idFundamentoLegalServicioSocial", "4"),
                                    new XAttribute("fundamentoLegalServicioSocial", "ART. 10 REGLAMENTO PARA LA PRESTACI�N DEL SERVICIO SOCIAL DE LOS ESTUDIANTES DE LAS INSTITUCIONES DE EDUCACI�N SUPERIOR EN LA REP�BLICA MEXICANA"),
                                    new XAttribute("idntidadFederativa", "09"),
                                    new XAttribute("entidadFederativa", "CIUDAD DE M�XICO")
                                 ),

                                 new XElement(defnamespace + "Antecedente",
                                    new XAttribute("institucionProcedencia", "PREPARATORIA NACIONAL NUMERO 9"),
                                    new XAttribute("idTipoEstudioAntecedente", "4"),
                                    new XAttribute("tipoEstudioAntecedente", "BACHILLERATO"),
                                    new XAttribute("idEntidadFederativa", "09"),
                                    new XAttribute("entidadFederativa", "CIUDAD DE MEXICO"),
                                    new XAttribute("fechaInicio", "2010 -12-13"),
                                    new XAttribute("fechaTerminacion", "2014 -11-13"),
                                    new XAttribute("noCedula", "") ////QUITAR
                                 )
                        )

            );

            //StringWriter writer = new StringWriter();
            //docXml.Save(writer);

            //return writer.GetStringBuilder().ToString();


            //ACTUALIZAR CAMPO DE SELLO
            string encodestring = "";
            XElement root = XElement.Parse(docXml.ToString());
            //foreach (var node in root.Descendants().Where(e => e.Attribute("sello")!=null))
            //{
            //    node.Attribute("sello").SetValue("VALOR NUEVOOOOOOO");
            //    //break;
            //}



            //XAttribute att = root.Attribute("sello");
            //att.SetValue("SELLOOOOOOOOOOOOOO");

            using (var sw = new MemoryStream())
            {
                using (var strw = new StreamWriter(sw, System.Text.UTF8Encoding.UTF8))
                {
                    root.Save(strw);
                    encodestring = System.Text.UTF8Encoding.UTF8.GetString(sw.ToArray());
                }
            }
            string xm = encodestring;

            Console.WriteLine(xm);

            //docXml.Save(@"C:\Users\Luis\Documents\Titulos\guardar.xml");
            //return docXml.ToString();
            // }
            Console.WriteLine("Xml generado");


            //AGREGAR NODO A XML
            byte[] encodedString = Encoding.UTF8.GetBytes(xm.ToString());
            MemoryStream ms = new MemoryStream(encodedString);
            ms.Flush();
            ms.Position = 0;

            XElement xy = XElement.Load(ms);


            //OBTENEMOS LA ETIQUETA RAIZ Y AÑADIMOS NUEVO ELEMENTO
            xy.Add(new XElement(defnamespace + "Autenticacion",
                                new XAttribute("version", 1.0),
                                new XAttribute("folioDigital", 10125635894),
                                new XAttribute("fechaAutenticacion", 2019-09-19),
                                new XAttribute("selloTitulo", "20121023SICERT0151610X0042086X00013"),
                                new XAttribute("noCertificadoAutoridad", "20121023SICERT0151610X0042086X00013"),
                                new XAttribute("selloAutenticacion", "20121023SICERT0151610X0042086X00013")));


            using (var sw = new MemoryStream())
            {
                using (var strw = new StreamWriter(sw, System.Text.UTF8Encoding.UTF8))
                {
                    xy.Save(strw);
                    encodestring = System.Text.UTF8Encoding.UTF8.GetString(sw.ToArray());
                }
            }
            string yy = encodestring;

            Console.WriteLine(yy);

            xy.Save(@"C:\Users\Luis\Documents\Titulos\guardar.xml");




            //Console.WriteLine("El sello es: " + sello);

            
            string xmlCorrecto = "UEsDBBQACAgIAFJEJE8AAAAAAAAAAAAAAAAeAAAAUmVzdWx0YWRvQ2FyZ2FUaXR1bG9zODQyNjYueGxz7VdLTBNBGP5mu6UgUFrkoQhkLRqlVAKa+CJaEDGSoCUF1MQa5LHBxkpNqRFP1tfBRBMTr1w4evFx0YPGyM2DiUYPJp7Qu4lGTTwo6z//7tKHopIoxsDfzOub/zX//DszffbUOz15p+o1cqgFDswYBcjLwEQmgwdwWtiMYRgSkrzGEv1XFEacfklo6MAotQmcyU2Fn1IFnMLWJX7NPptHj1xm/yBZT+A4BtmP4/OyLakUishcz+/KXXbN29QPSUF2zv8ZrQtnvyCfPuQ8J154nrjaYH7Dr0nrbXWKdb+hEsFJ5AOn9cFYfCQ6pC0E7WIfBoT0YQclzASN3VjJPpVyvZzrW8zxwOSjmWPCLSLbH9Zt5dUJHFJame8q1z6u3VQL3GOZV4w0oxyPZSTPXrOS2Im2RHQg9gcnatVq6laSE2t8dYG6uqb++pbIensQqa9VV1P0q7PmD4f14SPZTD64UJNmamzK1kNjybWONsyXy5WjzWLdAj82Av0S92u2ZWsYWW8ZtgHfBh/PtvaTaCPq6UeimXLZQjkSQQSwPduY6Vu2PXYtw2QwmFaxGQ20X2mjtvx3wjmS0yjj7fhkaBn5/0iTuLDxj7+HK/PEsQhxQXdKmux4Npj8n3Px+jnwwBy4fw684Dv8uqLCk3IYsvWm8rgtTancLk+5uEUK3JalnMZRPhcuwYtp+cAhjWF97FQsOTAcvwKVTAjcFNyqlfSV2ZfeF5yDvFOs0hZu39t5IES9jp7ett6+nkJgd0dPe7izu70ztL8Y2BPq6gxp7aH9veFQVxEwFB8d02OxeOP4iZgf6H2bPBWLa3pMH0omPoxGh+JaQh+JjiUT5Iimj0eT8bGBE/poUqejuXnbtk3NTU2bDSwjl/rIb1JIfft89+B+8RPXFMzzvVApm90XxWq7ydNiPheLqB5GCfe9PO8hnV9uvH++b7A72M+4n/EGrs8zkpI3hEVrZURhUEAUTKnSVjmVC8x9ketJOpwEWRH8UxAQASuO00G7FeijGdKquDM8dVDJVzzcV2Fdd6KEMZGDfeUIgNYiyRwJGonZkcLrtEcOGjmskWA5uQ4H2RcsZ9K7oDkvZZ2z3FJWxvYleTxBbRm2YqdSirv8gG5FmmosK0g/qKXxv/vMm5OEFVkHx8580+dZaZzP2W+6R+nLOUWJC7cVU7kLXsj7WN7GgL3TFVTonsMKKiupVFFZRaWaVw/UwvzPsET/nuQ+ZO7/jIUt7c/ioG9QSwcIaZ4JFaQDAAAAEAAAUEsBAhQAFAAICAgAUkQkT2meCRWkAwAAABAAAB4AAAAAAAAAAAAAAAAAAAAAAFJlc3VsdGFkb0NhcmdhVGl0dWxvczg0MjY2Lnhsc1BLBQYAAAAAAQABAEwAAADwAwAAAAA=";
            string plainTxt = xmlCorrecto;
            string encod = Encode.Base64Encode(plainTxt);
            byte[] y = Convert.FromBase64String(plainTxt);
            Console.WriteLine("ENCODING: " + y);

            string WSencode = encode;
            //string WSencode = "UEsDBBQACAgIAFJEJE8AAAAAAAAAAAAAAAAeAAAAUmVzdWx0YWRvQ2FyZ2FUaXR1bG9zODQyNjYueGxz7VdLTBNBGP5mu6UgUFrkoQhkLRqlVAKa+CJaEDGSoCUF1MQa5LHBxkpNqRFP1tfBRBMTr1w4evFx0YPGyM2DiUYPJp7Qu4lGTTwo6z//7tKHopIoxsDfzOub/zX//DszffbUOz15p+o1cqgFDswYBcjLwEQmgwdwWtiMYRgSkrzGEv1XFEacfklo6MAotQmcyU2Fn1IFnMLWJX7NPptHj1xm/yBZT+A4BtmP4/OyLakUishcz+/KXXbN29QPSUF2zv8ZrQtnvyCfPuQ8J154nrjaYH7Dr0nrbXWKdb+hEsFJ5AOn9cFYfCQ6pC0E7WIfBoT0YQclzASN3VjJPpVyvZzrW8zxwOSjmWPCLSLbH9Zt5dUJHFJame8q1z6u3VQL3GOZV4w0oxyPZSTPXrOS2Im2RHQg9gcnatVq6laSE2t8dYG6uqb++pbIensQqa9VV1P0q7PmD4f14SPZTD64UJNmamzK1kNjybWONsyXy5WjzWLdAj82Av0S92u2ZWsYWW8ZtgHfBh/PtvaTaCPq6UeimXLZQjkSQQSwPduY6Vu2PXYtw2QwmFaxGQ20X2mjtvx3wjmS0yjj7fhkaBn5/0iTuLDxj7+HK/PEsQhxQXdKmux4Npj8n3Px+jnwwBy4fw684Dv8uqLCk3IYsvWm8rgtTancLk+5uEUK3JalnMZRPhcuwYtp+cAhjWF97FQsOTAcvwKVTAjcFNyqlfSV2ZfeF5yDvFOs0hZu39t5IES9jp7ett6+nkJgd0dPe7izu70ztL8Y2BPq6gxp7aH9veFQVxEwFB8d02OxeOP4iZgf6H2bPBWLa3pMH0omPoxGh+JaQh+JjiUT5Iimj0eT8bGBE/poUqejuXnbtk3NTU2bDSwjl/rIb1JIfft89+B+8RPXFMzzvVApm90XxWq7ydNiPheLqB5GCfe9PO8hnV9uvH++b7A72M+4n/EGrs8zkpI3hEVrZURhUEAUTKnSVjmVC8x9ketJOpwEWRH8UxAQASuO00G7FeijGdKquDM8dVDJVzzcV2Fdd6KEMZGDfeUIgNYiyRwJGonZkcLrtEcOGjmskWA5uQ4H2RcsZ9K7oDkvZZ2z3FJWxvYleTxBbRm2YqdSirv8gG5FmmosK0g/qKXxv/vMm5OEFVkHx8580+dZaZzP2W+6R+nLOUWJC7cVU7kLXsj7WN7GgL3TFVTonsMKKiupVFFZRaWaVw/UwvzPsET/nuQ+ZO7/jIUt7c/ioG9QSwcIaZ4JFaQDAAAAEAAAUEsBAhQAFAAICAgAUkQkT2meCRWkAwAAABAAAB4AAAAAAAAAAAAAAAAAAAAAAFJlc3VsdGFkb0NhcmdhVGl0dWxvczg0MjY2Lnhsc1BLBQYAAAAAAQABAEwAAADwAwAAAAA=";
            string decode = Encode.Base64Decode(WSencode);
            Console.WriteLine("DECODE: " + decode);

            System.Text.Encoding encoding = null;



            byte[] x = Convert.FromBase64String(WSencode);
            /*string decodeText = encoding.GetString(x);
            string y = x.ToString();
            Console.WriteLine("BYTE: " + x);
            Console.WriteLine("String: " + decodeText);*/
            //File.WriteAllBytes(@"C:\Users\Luis\Documents\Titulos\ficheroExcel.xls", Convert.FromBase64String(WSencode));
            //File.WriteAllBytes(@"C:\Users\Luis\Documents\Titulos\ficheroExcel.txt", Convert.FromBase64String(WSencode));


            //GENERAR excell

            
            System.IO.File.WriteAllBytes(@"C:\Users\Luis\Documents\Titulos\ficheroExcel.xls",x);

            Console.ReadLine();


        }

        

        public static string ArmaPipes2(string selloxml)
        {
            string pipes = "||1.0|";
            //var doc = db.DOC_TITULO_PROFESION.Find(numd);
            //if (doc == null) { return "No existe numero de documento " + numd; }
            string numcer = "00001000000414093109";
            //var autoridad = db.CAT_FIRMATE_INTERNO.Find(doc.ID_FIRMANTE);
            //if (autoridad != null) { numcer = autoridad.NOCERTIFICADO_FIRMANTE; } else { numcer = "010203040506070809"; }
            // Genera UUID Codigo RFC 4122
            var EspacioId = Uuid.NewSqlSequentialId();
            DateTime fecha = new DateTime(2019, 09, 25);
            // var veo = Uuid.NewNamespaceUuid(EspacioId, "Hola Mundo");

            pipes += Uuid.NewNamespaceUuid(EspacioId, "19931111") + "|";
            pipes += fecha.ToString("yyyy-MM-ddThh:mm:ssZ") + "|"; //aaaa - mm - ddThh:mm: ss
            pipes += selloxml + "|";
            pipes += numcer + "|";
            // pipes += autoridad.CERTIFICADO_FIRMANTE + "|";
            pipes += "|";

            return pipes;
        }

        public static void CrearDirectorio() {

            string ruta = @"C:\Users\Luis\Documents\LoteAEnviarDgp";


            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
            else { }

        }

        public static void CrearDocumento(int idDoc, string strXml) {



            File.WriteAllText(@"C:\Users\Luis\Documents\LoteAEnviarDgp\"+idDoc+".xml", strXml);

        }


    }
}
