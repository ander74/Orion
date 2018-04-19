#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.PrintModel {

	public static class PdfTools {


		/// <summary>
		/// Devuelve una Tabla con dos columnas en las que se establecen los títulos pasados (izq y der).
		/// </summary>
		/// <param name="izquierda">Título que se colocará a la izquierda.</param>
		/// <param name="derecha">Título que se pondrá a la derecha.</param>
		/// <param name="estiloTabla">Estilo que se aplicará a la tabla.</param>
		/// <returns>Tabla con los títulos pasados.</returns>
		public static Table GetTablaTitulo(string izquierda, string derecha, Style estiloTabla = null) {
			// Creamos la tabla a devolver.
			Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50}));
			// Si hay un estilo, se aplica el mismo a la tabla.
			if (estiloTabla != null) tabla.AddStyle(estiloTabla);
			// Insertamos los dos títulos.
			tabla.AddCell(new Cell().Add(new Paragraph(izquierda)).SetTextAlignment(TextAlignment.LEFT).SetBorder(Border.NO_BORDER));
			tabla.AddCell(new Cell().Add(new Paragraph(derecha)).SetTextAlignment(TextAlignment.RIGHT).SetBorder(Border.NO_BORDER));
			// Devolvemos la tabla.
			return tabla;
		}




	}
}
