﻿using DanfeSharp.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DanfeSharp
{
    internal class FlexibleLine : DrawableBase
    {
        public List<DrawableBase> Elementos { get; private set; }

        /// <summary>
        /// Largura dos elementos, em porcentagem
        /// </summary>
        public List<float> ElementosLargurasP { get; private set; }

        public FlexibleLine()
        {
            Elementos = new List<DrawableBase>();
            ElementosLargurasP = new List<float>();
        }

        public FlexibleLine(float width, float height) : this()
        {
            Width = width;
            Height = height;
        }

        public virtual FlexibleLine ComElemento(DrawableBase db)
        {
            Elementos.Add(db ?? throw new ArgumentNullException(nameof(db)));
            return this;
        }

        public virtual FlexibleLine ComLarguras(params float[] elementosLarguras)
        {
            if (elementosLarguras.Length != Elementos.Count) throw new ArgumentException("A quantidade de larguras deve ser igual a de elementos.");
            
            float somaLarguras = elementosLarguras.Sum();
            if (somaLarguras > 100) throw new ArgumentOutOfRangeException("A soma das larguras passam de 100%.");

            var p = (100 - somaLarguras) / elementosLarguras.Where(x => x == 0).Count();

            for (int i = 0; i < elementosLarguras.Length; i++)
            {
                if (elementosLarguras[i] == 0)
                    elementosLarguras[i] = p;
            }

            ElementosLargurasP = elementosLarguras.ToList();
            return this;
        }

        public virtual FlexibleLine ComLargurasIguais()
        {
            float w = 100F / Elementos.Count;

            for (int i = 0; i < Elementos.Count; i++)
            {
                ElementosLargurasP.Add(w);
            }

            return this;
        }

        public void Posicionar()
        {         
            float wTotal = Elementos.Sum(s => s.Width);

            float x = X, y = Y;

            for (int i = 0; i < Elementos.Count; i++)
            {
                var e = Elementos[i];
                var ew = (Width * ElementosLargurasP[i]) / 100F;

                if (e is VerticalStack)
                {
                    e.Width = ew;
                }
                else
                {
                    e.SetSize(ew , Height);
                }

                e.SetPosition(x, y);           
                x += e.Width;
            }
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            Posicionar();

            foreach (var elemento in Elementos)
            {
                elemento.Draw(gfx);
            }

        }
    }
}
