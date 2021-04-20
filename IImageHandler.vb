Imports System.Drawing
Imports Emgu.CV.Structure
Imports Emgu.CV
Imports System.Windows.Forms
Imports System.Drawing.Imaging
Imports System.IO

Public Class IImageHandler

    Public Shared Function ClicaComponente(ByVal ImagemA As Bitmap) As Boolean
        Dim Retorno = False

        Dim bitmap As Bitmap = New Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
        Dim graphics As Graphics = Graphics.FromImage(TryCast(bitmap, Image))
        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size)
        Dim source As Image(Of Bgr, Byte) = New Image(Of Bgr, Byte)(bitmap)
        Dim template As Image(Of Bgr, Byte) = New Image(Of Bgr, Byte)(ImagemA)
        Dim imageToShow As Image(Of Bgr, Byte) = source.Copy()

        Using result As Image(Of Gray, Single) = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed)
            Dim minValues, maxValues As Double()
            Dim minLocations, maxLocations As Point()
            result.MinMax(minValues, maxValues, minLocations, maxLocations)

            If maxValues(0) > 0.9 Then

                Dim match As Rectangle = New Rectangle(maxLocations(0), template.Size)
                imageToShow.Draw(match, New Bgr(Color.Red), 3)
                Win32Utils.LeftMouseClick(match.X + (match.Width / 2), match.Y + (match.Height / 2))

            End If

        End Using

        Return Retorno
    End Function

    Public Shared Function LocalizaComponente(ByVal ImagemA As Bitmap) As Boolean
        Dim Retorno = False

        Dim bitmap As Bitmap = New Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
        Dim graphics As Graphics = Graphics.FromImage(TryCast(bitmap, Image))
        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size)
        Dim source As Image(Of Bgr, Byte) = New Image(Of Bgr, Byte)(bitmap)
        Dim template As Image(Of Bgr, Byte) = New Image(Of Bgr, Byte)(ImagemA)
        Dim imageToShow As Image(Of Bgr, Byte) = source.Copy()

        Using result As Image(Of Gray, Single) = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed)
            Dim minValues, maxValues As Double()
            Dim minLocations, maxLocations As Point()
            result.MinMax(minValues, maxValues, minLocations, maxLocations)

            If maxValues(0) > 0.9 Then
                Retorno = True
            End If
        End Using

        bitmap.Dispose()

        Return Retorno
    End Function

    Public Shared Function LocalizaComponente(ByVal ImagemA As Bitmap, ByVal ImagemB As Bitmap) As Boolean
        Dim Retorno = False

        Dim bitmap As Bitmap = ImagemB
        Dim graphics As Graphics = Graphics.FromImage(TryCast(bitmap, Image))
        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size)
        Dim source As Image(Of Bgr, Byte) = New Image(Of Bgr, Byte)(bitmap)
        Dim template As Image(Of Bgr, Byte) = New Image(Of Bgr, Byte)(ImagemA)
        Dim imageToShow As Image(Of Bgr, Byte) = source.Copy()

        Using result As Image(Of Gray, Single) = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed)
            Dim minValues, maxValues As Double()
            Dim minLocations, maxLocations As Point()
            result.MinMax(minValues, maxValues, minLocations, maxLocations)

            If maxValues(0) > 0.9 Then
                Retorno = True
            End If

        End Using
        bitmap.Dispose()

        Return Retorno
    End Function

    Public Shared Function PosicaoComponente(ByVal ImagemA As Bitmap) As Rectangle
        Dim Retorno As Rectangle

        Dim bitmap As Bitmap = New Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
        Dim graphics As Graphics = Graphics.FromImage(TryCast(bitmap, Image))
        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size)
        Dim source As Image(Of Bgr, Byte) = New Image(Of Bgr, Byte)(bitmap)
        Dim template As Image(Of Bgr, Byte) = New Image(Of Bgr, Byte)(ImagemA)
        Dim imageToShow As Image(Of Bgr, Byte) = source.Copy()

        Using result As Image(Of Gray, Single) = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed)
            Dim minValues, maxValues As Double()
            Dim minLocations, maxLocations As Point()
            result.MinMax(minValues, maxValues, minLocations, maxLocations)

            If maxValues(0) > 0.9 Then
                Dim match As Rectangle = New Rectangle(maxLocations(0), template.Size)
                imageToShow.Draw(match, New Bgr(Color.Red), 3)
                Retorno = match
            End If

        End Using

        Return Retorno
    End Function

    Public Shared Function VerificaComponente(ByVal ImagemA As Bitmap, ByVal Cor As Color) As Integer
        Dim Retorno = 0

        For y As Integer = 0 To ImagemA.Height - 1

            For x As Integer = 0 To ImagemA.Width - 1

                Dim PixelAtual = ImagemA.GetPixel(x, y)

                If PixelAtual = Cor Then
                    Retorno += 1
                End If

            Next

        Next

        Return Retorno
    End Function

    Public Shared Sub CapturarComponente(ByVal SourcePoint As Point, ByVal DestinationPoint As Point, ByVal SelectionRectangle As Rectangle, Dir As String)

        Try

            Dim bitmap As Bitmap = New Bitmap(SelectionRectangle.Width, SelectionRectangle.Height)
            Dim g As Graphics = Graphics.FromImage(bitmap)
            g.CopyFromScreen(SourcePoint, DestinationPoint, SelectionRectangle.Size)
            bitmap.Save(Dir)
            bitmap.Dispose()

        Catch ex As Exception

        End Try

    End Sub

End Class
