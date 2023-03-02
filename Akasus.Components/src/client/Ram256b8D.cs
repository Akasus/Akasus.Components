// File: Ram256b8Disp.cs
// Project: Akasus.Components
// Author: Akasus (akasus.nevelas@gmail.com)
// Description:


using System;
using System.Collections.Generic;
using System.Linq;
using Akasus.Display;
using LogicWorld.Interfaces;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Akasus.Components.Client
{
    public class Ram256b8D : ComponentClientCode
    {

        private RamDisplay display;
        private int[] data = new int[256];
        private bool hasData = false;


        //protected override void FrameUpdate()
        //{
        //    Logger.Info("Chip Enabled ? " + GetInputState(16 + 1));

        //    if (GetInputState(16 + 1) && display)
        //    {
        //        display.UpdateTexture();
        //        Logger.Info("Update Texture");
        //    }
        //}

        //protected override void DataUpdate()
        //{
        //    if (GetInputState(16 + 1))
        //    {
        //        ModClass.dispatcher.Invoke(() =>
        //        {
        //            display.Select = data.Address;
        //            display.Values = this.data.data.Select(e => (int)e).ToArray();
        //            display.UpdateTexture();
        //            Logger.Info("DataUpdate");
        //        });
        //    }
        //}

        protected override void DeserializeData(byte[] _data)
        {
            if (_data == null) return;
            if (_data.Length == 256)
            {
                data = _data.Select(e => (int)e).ToArray();
                Logger.Info("Data Init Response");
                hasData = true;
            }
            else if (_data.Length == 2 && display != null)
            {
                if (hasData && data != null)
                {
                    display.Values = data;
                    data = null;
                }

                Logger.Info("Update Byte");
                display.SetValue(_data[0], _data[1]);
                display.UpdateTexture();
            }
        }

        private byte inputToByte(int start)
        {
            byte tmp = 0;
            byte bitMask = 1;
            for (int i = start; i < (start + 8); i++)
            {
                if (GetInputState(i))
                {
                    tmp |= bitMask;
                }
                bitMask <<= 1;
            }
            return tmp;

            //return (byte)Inputs.Skip(start).Take(8).Select((e, i) => e.On ? 1 << i + 1 : 0).Sum();

        }

        protected override IList<IDecoration> GenerateDecorations()
        {
            var disp = MainClient.modAssets.LoadAsset<GameObject>("RAM Display");

            var inst = Object.Instantiate(disp);

            display = inst.GetComponent<RamDisplay>();
            if (display == null)
                throw new NullReferenceException("Couldn't find RAMDisplay!");

            if (hasData)
            {
                display.Values = data;
                data = null;
                display.UpdateTexture();
                hasData = false;
            }
            //inst.SetActive(false);
            //display.AddressLines = 2;
            //display.BusLines = 2;
            //display.AutoScroll = true;
            //display.AutoScrollBehaviour = RamDisplay.AutoScollFollowBehaviour.Paged;
            //inst.SetActive(true);
            Logger.Info("Generate Decorations");

            return new List<IDecoration>()
            {
                new Decoration()
                {
                    DecorationObject = inst,
                    AutoSetupColliders = false,
                }
            };
        }
    }
}