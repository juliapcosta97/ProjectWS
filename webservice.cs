using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Services;
using System.Xml;

/// <summary>
/// Descrição resumida de WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que esse serviço da web seja chamado a partir do script, usando ASP.NET AJAX, remova os comentários da linha a seguir. 
// [System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{

    public WebService()
    {

        //Remova os comentários da linha a seguir se usar componentes designados 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string assinaturaDigital()
    {
        XmlDocument xml = new XmlDocument();

        //Cria arquivo XML com a confirmação de registro de ordem de produção
        criaXml();

        //Carrega arquivo XML criado com a confirmação de registro
        String arquivo = @"C:\Users\julia\Desktop\XML\teste.xml";
        xml.Load(arquivo);

        //Senha para leitura do arquivo que contém o certificado digital
        String senha = "1234567890";

        //Caminho arquivo de certificado digital
        String caminhoCertificado = @"C:\Users\julia\Desktop\wibit-test-cert.pfx";

        //Busca certificado digital
        X509Certificate2 certificado = new X509Certificate2(File.ReadAllBytes(caminhoCertificado), senha );

        //Assina arquivo XML criado com o certificado digital 
        assinaDocumentoComCertificado(xml,certificado);

        return "Olá, Mundo";
    }

    public static void assinaDocumentoComCertificado(XmlDocument doc, X509Certificate2 certificado)
    {
        SignedXml signedXml = new SignedXml(doc);
        signedXml.SigningKey = certificado.PrivateKey;
        Reference reference = new Reference();
        reference.Uri = "";
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        signedXml.AddReference(reference);

        KeyInfo keyinfo = new KeyInfo();
        keyinfo.AddClause(new KeyInfoX509Data(certificado));

        signedXml.KeyInfo = keyinfo;
        signedXml.ComputeSignature();

        XmlElement xmlSig = signedXml.GetXml();

        doc.DocumentElement.AppendChild(doc.ImportNode(xmlSig, true));

        File.WriteAllText(@"C:\Users\julia\Desktop\XML\teste.xml",doc.OuterXml);
    }

    private static void criaXml()
    {

        XmlTextWriter writer = new XmlTextWriter(@"C:\Users\julia\Desktop\XML\teste.xml", null);

        //inicia o documento xml
        writer.WriteStartDocument();

        //define a indentação do arquivo
        writer.Formatting = Formatting.Indented;

        //escreve o elmento raiz
        writer.WriteStartElement("ConfirmaOrdemProducao");

        //Escreve os sub-elementos
        writer.WriteElementString("id", "1");
        writer.WriteElementString("ordemProducao", "123");
        writer.WriteElementString("resultadoProcessamentoGrafica", "OK");
        writer.WriteElementString("mensagemErro", "");
        // encerra o elemento raiz
        writer.WriteEndElement();
        //Escreve o XML para o arquivo e fecha o objeto escritor
        writer.Close();
    }