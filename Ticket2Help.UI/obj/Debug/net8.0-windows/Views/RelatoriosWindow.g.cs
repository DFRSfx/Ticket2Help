﻿#pragma checksum "..\..\..\..\Views\RelatoriosWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C58F515C33355F8D8AD68789E43A806960158BB9"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações a este ficheiro poderão provocar um comportamento incorrecto e perder-se-ão se
//     o código for regenerado.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Ticket2Help.UI.Views {
    
    
    /// <summary>
    /// RelatoriosWindow
    /// </summary>
    public partial class RelatoriosWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 169 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker DatePickerInicio;
        
        #line default
        #line hidden
        
        
        #line 177 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker DatePickerFim;
        
        #line default
        #line hidden
        
        
        #line 183 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnGerarRelatorio;
        
        #line default
        #line hidden
        
        
        #line 191 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnExportarExcel;
        
        #line default
        #line hidden
        
        
        #line 200 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel LoadingIndicator;
        
        #line default
        #line hidden
        
        
        #line 253 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LblTotalRegistrosHeader;
        
        #line default
        #line hidden
        
        
        #line 264 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DataGridRelatorio;
        
        #line default
        #line hidden
        
        
        #line 336 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel NoDataPanel;
        
        #line default
        #line hidden
        
        
        #line 370 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LblTotalRegistros;
        
        #line default
        #line hidden
        
        
        #line 381 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LblPeriodoSelecionado;
        
        #line default
        #line hidden
        
        
        #line 392 "..\..\..\..\Views\RelatoriosWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LblUltimaActualizacao;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.5.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Ticket2Help.UI;component/views/relatorioswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\RelatoriosWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.5.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.DatePickerInicio = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 2:
            this.DatePickerFim = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 3:
            this.BtnGerarRelatorio = ((System.Windows.Controls.Button)(target));
            
            #line 187 "..\..\..\..\Views\RelatoriosWindow.xaml"
            this.BtnGerarRelatorio.Click += new System.Windows.RoutedEventHandler(this.BtnGerarRelatorio_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.BtnExportarExcel = ((System.Windows.Controls.Button)(target));
            
            #line 195 "..\..\..\..\Views\RelatoriosWindow.xaml"
            this.BtnExportarExcel.Click += new System.Windows.RoutedEventHandler(this.BtnExportarExcel_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.LoadingIndicator = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.LblTotalRegistrosHeader = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.DataGridRelatorio = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 8:
            this.NoDataPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 9:
            this.LblTotalRegistros = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.LblPeriodoSelecionado = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.LblUltimaActualizacao = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

