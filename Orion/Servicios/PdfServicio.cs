#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using iText.Forms;
using iText.Forms.Fields;
using iText.Layout.Element;
using iText.Layout.Renderer;

namespace Orion.Servicios {
    public class PdfServicio {

        // ====================================================================================================
        #region CELL RENDERERS
        // ====================================================================================================

        /// <summary>
        /// Añade un TextBox a una celda de una tabla. Se debe construir pasando la celda, 
        /// el nombre del campo, el margen del mismo sobre la celda y la alineación
        /// </summary>
        public class TextFieldRenderer : CellRenderer {

            public String fieldName;
            public float margenExterior;
            public int justificacion;
            public float fontSize;
            public string valor = "";


            public TextFieldRenderer(Cell modelElement, String fieldName, float margenExterior, float fontSize, int justificacion = PdfFormField.ALIGN_LEFT) : base(modelElement) {

                this.fieldName = fieldName;
                this.margenExterior = margenExterior;
                this.justificacion = justificacion;
                this.fontSize = fontSize;
            }


            public TextFieldRenderer(Cell modelElement, String fieldName, string valor, float margenExterior, float fontSize, int justificacion = PdfFormField.ALIGN_LEFT) : base(modelElement) {

                this.fieldName = fieldName;
                this.margenExterior = margenExterior;
                this.justificacion = justificacion;
                this.fontSize = fontSize;
                this.valor = valor;
            }


            /// <summary>
            /// Mantiene un margen de 2, fuente de 10 puntos y alineación a la izquierda.
            /// </summary>
            public TextFieldRenderer(Cell modelElement, String fieldName) : base(modelElement) {

                this.fieldName = fieldName;
                this.margenExterior = 2;
                this.justificacion = PdfFormField.ALIGN_LEFT;
                this.fontSize = 10;
            }


            override public void Draw(DrawContext drawContext) {
                base.Draw(drawContext);
                iText.Kernel.Geom.Rectangle cuadro = GetOccupiedAreaBBox();
                cuadro.MoveUp(margenExterior).MoveRight(margenExterior);
                cuadro.SetHeight(cuadro.GetHeight() - (margenExterior * 2));
                cuadro.SetWidth(cuadro.GetWidth() - (margenExterior * 2));
                PdfTextFormField field = PdfFormField.CreateText(drawContext.GetDocument(), cuadro, fieldName, valor);
                field.SetJustification(justificacion);
                field.SetVisibility(PdfFormField.VISIBLE);
                field.SetFontSize(fontSize);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(drawContext.GetDocument(), true);
                form.AddField(field);
            }
        }



        #endregion
        // ====================================================================================================


    }
}
