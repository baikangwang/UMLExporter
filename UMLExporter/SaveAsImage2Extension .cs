using System.Drawing.Imaging;
using System.Linq;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Microsoft.VisualStudio.ArchitectureTools.Extensibility.Presentation;
using Microsoft.VisualStudio.ArchitectureTools.Extensibility.Uml;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;

namespace UMLExporter
{
    // Custom context menu command extension
    // See http://msdn.microsoft.com/en-us/library/ee329481(VS.100).aspx
    [Export(typeof (ICommandExtension))]
    [ClassDesignerExtension]
    [UseCaseDesignerExtension]
    [SequenceDesignerExtension]
    [ComponentDesignerExtension]
    [ActivityDesignerExtension]
    internal class SaveAsImage2Extension : ICommandExtension
    {
        [Import(typeof (IDiagramContext))]
        public IDiagramContext DiagramContext { get; set; }

        public string Text
        {
            get { return "Save as image..."; }
        }

        public void Execute(IMenuCommand command)
        {
            var dslDiagram = DiagramContext.CurrentDiagram.GetObject<Diagram>();
            if (dslDiagram != null)
            {
                var dialog = new SaveFileDialog
                                 {
                                     AddExtension = true,
                                     DefaultExt = "image.bmp",
                                     Filter =
                                         "Bitmap ( *.bmp )|*.bmp|" + "JPEG File ( *.jpg )|*.jpg|" +
                                         "Enhanced Metafile (*.emf )|*.emf|" + "Portable Network Graphic ( *.png )|*.png",
                                     FilterIndex = 1,
                                     Title = "Save Diagram to Image"
                                 };
                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
                {
                    var bitmap = dslDiagram.CreateBitmap(dslDiagram.NestedChildShapes,
                                                         Diagram.CreateBitmapPreference.FavorClarityOverSmallSize);
                    bitmap.Save(dialog.FileName, GetImageType(dialog.FilterIndex));
                }
            }
        }

        public void QueryStatus(IMenuCommand command)
        {
            if (DiagramContext.CurrentDiagram != null && DiagramContext.CurrentDiagram.ChildShapes.Any())
            {
                command.Enabled = true;
            }
            else
            {
                command.Enabled = false;
            }
        }

        private static ImageFormat GetImageType(int filterIndex)
        {
            var result = ImageFormat.Bmp;
            switch (filterIndex)
            {
                case 2:
                    result = ImageFormat.Jpeg;
                    break;
                case 3:
                    result = ImageFormat.Emf;
                    break;
                case 4:
                    result = ImageFormat.Png;
                    break;
            }
            return result;
        }
    }
}


