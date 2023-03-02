// File: Ram256b8D.cs
// Project: EcconiaCPUServerComponents
// Author: Akasus (akasus.nevelas@gmail.com)
// Description:

using System;
using LogicAPI.Server.Components;
using LogicAPI.WorldDataMutations;
using LogicWorld.Server;
using LogicWorld.Server.Managers;

namespace Akasus.Components.Server
{
    public class Ram256b8D : LogicComponent
    {
        private static readonly IWorldUpdates worldUpdater;

        //Contains the in memory stored bytes:
        private byte[] data = new byte[256];

        private byte address;
        private bool chipEnable;
        private bool readWrite;

        static Ram256b8D()
        {
            //World updater only exists once while runtime anyway. So lets keep it cached statically.
            worldUpdater = Program.Get<IWorldUpdates>();
        }

        //Set this to true, so that LogicWorld knows, that it has to serialize this component before saving.
        public override bool HasPersistentValues => true;
        //No need to override the SavePersistentValuesToCustomData method, since there is no CustomData object.

        protected override void DeserializeData(byte[] customDataArray)
        {
            if (customDataArray == null)
            {
                //No need to initialize any of this mod. Is it null because its a new component?
                return;
            }
            if (customDataArray.Length == 1)
            {
                //This is a message from a dear client, probably requesting a broadcast.
                if (customDataArray[0] == 0)
                {
                    //Indeed a broadcast request:
                    worldUpdater.QueueMutationToBeSentToClient(new WorldMutation_UpdateComponentCustomData()
                    {
                        AddressOfTargetComponent = Address,
                        NewCustomData = data,
                    });
                    return; //Done here.
                }
                else
                {
                    throw new Exception("Invalid custom data message sent by client, content: " + customDataArray[0]);
                }
            }

            if (customDataArray.Length != 256)
                throw new ArgumentException("customData isn't the right size!");

            data = customDataArray;
        }

        protected override byte[] SerializeCustomData()
        {
            return data;
        }

        protected override void DoLogicUpdate()
        {
            //Read all current inputs (with level - 1):

            chipEnable = Inputs[17].On;
            readWrite = Inputs[16].On;
            
            address = chipEnable? inputToByte(0) : (byte)0;
            byte dataIn = inputToByte(8);

            Logger.Info("SERVER: Address: " + address);
            Logger.Info("SERVER: Data: " + dataIn);
            Logger.Info($"SERVER: R/W: {readWrite}   CE: {chipEnable}");


            //Perform actions:
            //We are using Level 3 as input, because we calculate the 4th tick.
            if (chipEnable && readWrite)
            {
                data[address] = dataIn;
            }
            setOutput(chipEnable && !readWrite ? data[address] : (byte)0);


            Logger.Info("SERVER: Current RAM DATA: " + BitConverter.ToString(data, 0, 256));
            worldUpdater.QueueMutationToBeSentToClient(new WorldMutation_UpdateComponentCustomData()
            {
                AddressOfTargetComponent = Address,
                NewCustomData = new byte[] { address, data[address] },
            });
        }

        private void setOutput(byte value)
        {
            for (int i = 0; i < 8; i++)
            {
                Outputs[i].On = (value & 1) != 0;
                value >>= 1;
            }
        }

        private byte inputToByte(int start)
        {
            byte tmp = 0;
            byte bitMask = 1;
            for (int i = start; i < (start + 8); i++)
            {
                if (Inputs[i].On)
                {
                    tmp |= bitMask;
                }
                bitMask <<= 1;
            }
            return tmp;

            //return (byte)Inputs.Skip(start).Take(8).Select((e, i) => e.On ? 1 << i + 1 : 0).Sum();

        }
    }
}