Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Threading

Public Class IVivo360Handler

#Region "Vivo360 Posts"




    Public Shared Function Vivo360AtualizacaoCadastral(DadosPrimeiroNome As String, DadosSobrenome As String, DadosNomeInteiro As String, DadosTerminal As String, DadosDocumento As String, DadosDataNascimento As String, DadosNomeMae As String) As String
        Dim Retorno As String = ""
        Try

            Dim codigoCliente1 = ""
            Dim codigoCliente2 = ""
            Dim codigoCliente3 = ""
            Dim codigoCliente4 = ""

            Dim reqLoginVivo360 As String = String.Format(Querys.LocalizaRequestByMethod("autenticaUsuario"), Variaveis.SenhaVivo360, Variaveis.UsuarioVivo360)

            Dim rspLoginVivo360 = SendRequest("http://vivo360.vivo.com.br/vivo360/modules/login/crux.rpc?locale=pt_BR", reqLoginVivo360, "text/x-gwt-rpc; charset=UTF-8", "POST", Reflection.MethodBase.GetCurrentMethod.Name)

            Dim boolLoginVivo360 = rspLoginVivo360.StartsWith("//OK")

            If boolLoginVivo360 Then

                Dim reqCodigosCliente As String = String.Format(Querys.LocalizaRequestByMethod("buscarListaPessoas"), DadosTerminal)

                Dim rspCodigosCliente = SendRequest("http://vivo360.vivo.com.br/vivo360/modules/lojas/crux.rpc?locale=pt_BR", reqCodigosCliente, "text/x-gwt-rpc; charset=UTF-8", "POST", Reflection.MethodBase.GetCurrentMethod.Name)

                Dim boolCodigosCliente = rspCodigosCliente IsNot Nothing And rspCodigosCliente.StartsWith("//OK")

                If boolCodigosCliente Then

                    Dim numeroCodigo = 1
                    Dim resp1 = rspCodigosCliente.Substring(0, rspCodigosCliente.IndexOf("java.util"))
                    resp1 = resp1.Replace("//OK[", "")
                    For Each rsp In resp1.Split(",")
                        If rsp.Length > 5 Then
                            If numeroCodigo = 1 Then
                                codigoCliente1 = rsp.Replace("'", "")
                            ElseIf numeroCodigo = 2 Then
                                codigoCliente2 = rsp.Replace("'", "")
                            ElseIf numeroCodigo = 3 Then
                                codigoCliente3 = rsp.Replace("'", "")
                            End If
                            numeroCodigo += 1
                        End If
                    Next

                    Dim reqCodigosCliente2 As String = String.Format(Querys.LocalizaRequestByMethod("buscarResumoPessoa"), codigoCliente3, codigoCliente1)

                    Dim rspCodigosCliente2 = SendRequest("http://vivo360.vivo.com.br/vivo360/modules/lojas/crux.rpc?locale=pt_BR", reqCodigosCliente2, "text/x-gwt-rpc; charset=UTF-8", "POST", Reflection.MethodBase.GetCurrentMethod.Name)

                    Dim boolCodigosCliente2 = rspCodigosCliente2 IsNot Nothing And rspCodigosCliente2.StartsWith("//OK")

                    If boolCodigosCliente2 Then

                        Dim numeroCodigo2 = 0
                        For Each rsp In rspCodigosCliente2.Split(",")
                            If rsp.Length > 5 Then
                                If numeroCodigo2 = 7 Then
                                    codigoCliente4 = rsp.Replace("'", "")
                                    Exit For
                                End If
                                numeroCodigo2 += 1
                            End If
                        Next

                        Dim DadosDataNascimentoGwt = DatasV360.DateTimeToDateGwt(DadosDataNascimento)

                        Dim reqDadosCadastrais = String.Format(Querys.LocalizaRequestByMethod("atualizarDadosPessoaFisica"), DadosPrimeiroNome, DadosNomeInteiro, DadosNomeMae, DadosSobrenome, DadosTerminal, codigoCliente1, DadosDataNascimentoGwt, codigoCliente3, codigoCliente4)

                        Dim rspDadosCadastrais = SendRequest("http://vivo360.vivo.com.br/vivo360/modules/manterdadoscliente/crux.rpc?locale=pt_BR", reqDadosCadastrais, "text/x-gwt-rpc; charset=UTF-8", "POST", Reflection.MethodBase.GetCurrentMethod.Name)

                        Dim boolDadosCadastrais = rspDadosCadastrais.StartsWith("//OK")

                        If boolDadosCadastrais Then

                            Dim reqDocumento = String.Format(Querys.LocalizaRequestByMethod("alterarDocumento"), DadosDocumento, DadosTerminal, codigoCliente1, codigoCliente2, codigoCliente3)

                            Dim rspDocumento = SendRequest("http://vivo360.vivo.com.br/vivo360/modules/manterdadoscliente/crux.rpc?locale=pt_BR", reqDocumento, "text/x-gwt-rpc; charset=UTF-8", "POST", Reflection.MethodBase.GetCurrentMethod.Name)

                            Dim boolDocumento = rspDocumento.StartsWith("//OK")

                            If boolDocumento Then

                                Retorno = ""

                            Else
                                Retorno = "Não foi possível realizar o processo de atualização do documento do cliente Vivo 360"
                            End If
                        Else
                            Retorno = "Não foi possível realizar o processo de atualização cadastral do cliente Vivo 360"
                        End If
                    Else
                        Retorno = "Não foi possível realizar o processo de consulta do cliente 2 Vivo 360"
                    End If
                Else
                    Retorno = "Não foi possível realizar o processo de consulta do cliente Vivo 360"
                End If
            Else
                Retorno = "Não foi possível realizar a autenticação no Vivo 360"
            End If




        Catch ex As Exception

        End Try
        Return Retorno
    End Function



#End Region






#Region "HTTPUtils"

    Public Shared Function SendRequest(uri As String, bodyPayload As String, contentType As String, httpMethod As String, apiMethod As String) As String
        Dim Response As String = ""

        Dim Tentativas = 1

InicioRequest:

        Try

            Dim HttpReq As HttpWebRequest = DirectCast(WebRequest.Create(uri), HttpWebRequest)
            HttpReq.Timeout = Timeout.Infinite
            HttpReq.KeepAlive = True
            HttpReq.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*"
            HttpReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.3; WOW64; Trident/7.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729)"
            HttpReq.Headers("Accept-Enconding") = "gzip, deflate"
            HttpReq.Headers("Accept-Language") = "pt-br"
            HttpReq.Headers("UA-CPU") = "x86"
            HttpReq.Headers.Add(HttpRequestHeader.CacheControl, "no-cache")
            HttpReq.PreAuthenticate = True
            HttpReq.AllowAutoRedirect = True
            HttpReq.ServicePoint.Expect100Continue = False
            HttpReq.Referer = ""


            HttpReq.CookieContainer = New CookieContainer()
            If TodosCookies.Count > 0 Then
                For Each Cookie As Cookie In TodosCookies
                    HttpReq.CookieContainer.Add(Cookie)
                Next
            End If

            If bodyPayload IsNot Nothing AndAlso bodyPayload.Trim().Length > 0 Then
                Dim data As Byte() = System.Text.Encoding.UTF8.GetBytes(bodyPayload)
                HttpReq.Method = "POST"
                HttpReq.ContentType = "text/x-gwt-rpc; charset=UTF-8"
                HttpReq.ContentLength = data.Length
                Dim req_stream As Stream = HttpReq.GetRequestStream()
                req_stream.Write(data, 0, data.Length)
                req_stream.Close()
            End If

            Dim HttpRes As HttpWebResponse = DirectCast(HttpReq.GetResponse(), HttpWebResponse)
            Dim readStream As New StreamReader(HttpRes.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"))
            Response = readStream.ReadToEnd()
            readStream.Close()
            AdicionarCookies(HttpRes.Cookies)
            HttpRes.Close()

        Catch ex As WebException
            Response = RetornaWebExTratada(ex)
        End Try

        Querys.InsereLogRequestsRoboVivoNext(apiMethod, bodyPayload, Response)

        If Response.ToString().StartsWith("//EX") Then
            If Tentativas < 3 Then
                Tentativas += 1
                GoTo InicioRequest
            End If
        End If

        Return Response
    End Function


    Public Shared Function RetornaWebExTratada(webEx As WebException) As String
        Dim Retorno As String = ""
        Try

            Retorno = New StreamReader(webEx.Response.GetResponseStream()).ReadToEnd()

        Catch ex As Exception

        End Try
        Return Retorno
    End Function



    Public Shared TodosCookies As New CookieCollection()

    Public Shared Sub AdicionarCookies(RetonoChamada As CookieCollection)
        If RetonoChamada Is Nothing Then
            Return
        End If

        For Each Cookie As Cookie In RetonoChamada
            If TodosCookies(Cookie.Name) IsNot Nothing Then
                TodosCookies(Cookie.Name).Value = Cookie.Value
                If TodosCookies(Cookie.Name).Domain <> Cookie.Domain Then
                    TodosCookies(Cookie.Name).Domain = Cookie.Domain
                End If
            Else
                TodosCookies.Add(Cookie)
            End If
        Next
    End Sub

#End Region

End Class
