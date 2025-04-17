namespace squispeS2T2
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            seguimiento1.TextChanged += OnTextChangedOnlyNumeric;
            seguimiento2.TextChanged += OnTextChangedOnlyNumeric;
            examen1.TextChanged += OnTextChangedOnlyNumeric;
            examen2.TextChanged += OnTextChangedOnlyNumeric;
        }

        private void OnTextChangedOnlyNumeric(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (entry == null) return;

            if (!double.TryParse(entry.Text, out _) && !string.IsNullOrEmpty(entry.Text))
            {
                entry.Text = e.OldTextValue;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (Estudiantes.SelectedItem == null ||
            string.IsNullOrWhiteSpace(seguimiento1.Text) ||
            string.IsNullOrWhiteSpace(examen1.Text) ||
            string.IsNullOrWhiteSpace(seguimiento2.Text) ||
            string.IsNullOrWhiteSpace(examen2.Text))
            {
                await DisplayAlert("Error", "Por favor, complete todos los campos.", "OK");
                return;
            }

            try
            {
                string estudiante = Estudiantes.SelectedItem.ToString();
                double seg1 = double.Parse(seguimiento1.Text);
                double ex1 = double.Parse(examen1.Text);
                double seg2 = double.Parse(seguimiento2.Text);
                double ex2 = double.Parse(examen2.Text);

                if (seg1 < 0 || seg1 > 10 || ex1 < 0 || ex1 > 10 || seg2 < 0 || seg2 > 10 || ex2 < 0 || ex2 > 10)
                {
                    await DisplayAlert("Error", "Las notas deben estar entre 0 y 10.", "OK");
                    return;
                }

                double parcial1 = (seg1 * 0.3) + (ex1 * 0.2);
                double parcial2 = (seg2 * 0.3) + (ex2 * 0.2);
                double notaFinal = parcial1 + parcial2;

                string estado = notaFinal >= 7 ? "APROBADO" :
                                (notaFinal >= 5 ? "COMPLEMENTARIO" : "REPROBADO");

                string fecha = fechaSelect.Date.ToString("dd/MM/yyyy");

                string resultado = $"Nombre: {estudiante}\nFecha: {fecha}\n" +
                                   $"Nota Parcial 1: {parcial1:F2}\n" +
                                   $"Nota Parcial 2: {parcial2:F2}\n" +
                                   $"Nota Final: {notaFinal:F2}\n" +
                                   $"Estado: {estado}";

                await DisplayAlert("Resultados", resultado, "OK");
                LimpiarFormulario();
                await GuardarEnArchivo(resultado, estudiante);

            }
            catch
            {
                await DisplayAlert("Error", "Asegúrese de ingresar valores válidos.", "OK");
            }
        }

        private async Task GuardarEnArchivo(string contenido, string estudiante)
        {
            string carpeta = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            try
            {
                string nombreLimpio = estudiante.Replace(" ", "_");
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string nombreArchivo = $"{nombreLimpio}_{timestamp}.txt";
                string rutaArchivo = Path.Combine(carpeta, nombreArchivo);

                using StreamWriter writer = new(rutaArchivo, append: false);
                await writer.WriteLineAsync(contenido);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo guardar el archivo: {ex.Message}", "OK");
            }
        }

        private void LimpiarFormulario()
        {
            Estudiantes.SelectedIndex = -1;
            seguimiento1.Text = string.Empty;
            examen1.Text = string.Empty;
            seguimiento2.Text = string.Empty;
            examen2.Text = string.Empty;
            fechaSelect.Date = DateTime.Now;
        }

    }

}
