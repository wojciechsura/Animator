using Animator.Engine.Base;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Utilities;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Animator.Engine.Base.Persistence.Types;

namespace Animator.Extensions.Nonconformist.Elements
{
    public class SymbolicCode : Visual
    {
        private const float IncIndentProbability = 0.2f;
        private const float DecIndentProbability = 0.4f;
        private const float SpecialCharProbability = 0.3f;
        private const float LineToMaxWordLengthRatio = 0.4f;

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            int lineCount = (int)Math.Floor((Height / CharHeight) / 2);
            int maxLineLength = (int)Math.Floor(Width / CharHeight);

            var random = new Random(Seed);

            int indent = 0;

            using var textBrush = new System.Drawing.SolidBrush(TextColor);
            using var specialCharBrush = new System.Drawing.SolidBrush(SpecialCharColor);
            using var semicolonBrush = new System.Drawing.SolidBrush(SemicolonColor);

            int maxWordLength = (int)(maxLineLength * LineToMaxWordLengthRatio);

            for (int i = 0; i < lineCount; i++)
            {
                if (Indent)
                {
                    if (indent < MaxIndent && random.Next() / (float)int.MaxValue < IncIndentProbability)
                    {
                        indent++;
                    }
                    else if (indent > 0 && random.Next() / (float)int.MaxValue < DecIndentProbability)
                    {
                        indent--;
                    }
                }

                indent = Math.Min(indent, (lineCount - 1) - i);

                int lineLength = random.Next(1, Math.Max(0, (SpecialCharacters ? maxLineLength - 2 : maxLineLength) - 5 * indent));

                if (!Words)
                {
                    buffer.Graphics.FillRectangle(textBrush, 
                        indent * 5 * CharHeight, 
                        i * 2 * CharHeight, 
                        lineLength * CharHeight, 
                        CharHeight);
                }
                else
                {
                    int pos = 0;
                    bool lastWasSpecial = true;

                    while (pos < lineLength)
                    {
                        if (SpecialCharacters && pos <= lineLength - 3 && !lastWasSpecial && random.Next() / (float)int.MaxValue < SpecialCharProbability)
                        {
                            buffer.Graphics.FillRectangle(specialCharBrush, 
                                (indent * 5 + pos) * CharHeight, 
                                i * 2 * CharHeight, 
                                CharHeight, 
                                CharHeight);

                            pos += 1 + 1;

                            lastWasSpecial = true;
                        }
                        else
                        {
                            int wordLength = random.Next(1, Math.Min(lineLength - pos, maxWordLength));

                            if (pos + wordLength >= lineLength - 1)
                                wordLength = lineLength - pos;

                            buffer.Graphics.FillRectangle(textBrush, 
                                (indent * 5 + pos) * CharHeight, 
                                i * 2 * CharHeight, 
                                wordLength * CharHeight, 
                                CharHeight);

                            pos += wordLength + 1;

                            lastWasSpecial = false;
                        }
                    }
                }

                if (SpecialCharacters)
                {
                    buffer.Graphics.FillRectangle(semicolonBrush, 
                        (indent * 5 + lineLength + 1) * CharHeight, 
                        i * 2 * CharHeight, 
                        CharHeight, 
                        CharHeight);
                }
            }
        }

        // Public properties --------------------------------------------------


        #region Seed managed property

        public int Seed
        {
            get => (int)GetValue(SeedProperty);
            set => SetValue(SeedProperty, value);
        }

        public static readonly ManagedProperty SeedProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(Seed),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion

        #region Height managed property

        public float Height
        {
            get => (float)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public static readonly ManagedProperty HeightProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(Height),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 100.0f });

        #endregion

        #region Width managed property

        public float Width
        {
            get => (float)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static readonly ManagedProperty WidthProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(Width),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 100.0f });

        #endregion

        #region CharHeight managed property

        public float CharHeight
        {
            get => (float)GetValue(CharHeightProperty);
            set => SetValue(CharHeightProperty, value);
        }

        public static readonly ManagedProperty CharHeightProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(CharHeight),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 1.0f });

        #endregion

        #region Words managed property

        public bool Words
        {
            get => (bool)GetValue(WordsProperty);
            set => SetValue(WordsProperty, value);
        }

        public static readonly ManagedProperty WordsProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(Words),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

        #endregion

        #region SpecialCharacters managed property

        public bool SpecialCharacters
        {
            get => (bool)GetValue(SpecialCharactersProperty);
            set => SetValue(SpecialCharactersProperty, value);
        }

        public static readonly ManagedProperty SpecialCharactersProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(SpecialCharacters),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

        #endregion

        #region Indent managed property

        public bool Indent
        {
            get => (bool)GetValue(IndentProperty);
            set => SetValue(IndentProperty, value);
        }

        public static readonly ManagedProperty IndentProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(Indent),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

        #endregion

        #region MaxIndent managed property

        public int MaxIndent
        {
            get => (int)GetValue(MaxIndentProperty);
            set => SetValue(MaxIndentProperty, value);
        }

        public static readonly ManagedProperty MaxIndentProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(MaxIndent),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 3 });

        #endregion

        #region TextColor managed property

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly ManagedProperty TextColorProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(TextColor),
            typeof(Color),
            new ManagedSimplePropertyMetadata { DefaultValue = Color.FromArgb(0, 0, 0) });

        #endregion

        #region SpecialCharColor managed property

        public Color SpecialCharColor
        {
            get => (Color)GetValue(SpecialCharColorProperty);
            set => SetValue(SpecialCharColorProperty, value);
        }

        public static readonly ManagedProperty SpecialCharColorProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(SpecialCharColor),
            typeof(Color),
            new ManagedSimplePropertyMetadata { DefaultValue = Color.FromArgb(255, 128, 0) });

        #endregion

        #region SemicolonColor managed property

        public Color SemicolonColor
        {
            get => (Color)GetValue(SemicolonColorProperty);
            set => SetValue(SemicolonColorProperty, value);
        }

        public static readonly ManagedProperty SemicolonColorProperty = ManagedProperty.Register(typeof(SymbolicCode),
            nameof(SemicolonColor),
            typeof(Color),
            new ManagedSimplePropertyMetadata { DefaultValue = Color.FromArgb(0, 128, 255) });

        #endregion
    }
}
