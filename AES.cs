//make an Object Type for Key potentially set max length to 128bit
using System;
using Program;
namespace AES{
    class AES{
        //const string plaintext = "Two One Nine Two";
        //const string key = "Thats my Kung Fu";
         int[,] rcon = new int[11,4]{
            {1,0,0,0},  {2,0,0,0},  {4,0,0,0},  {8,0,0,0},
            {16,0,0,0}, {32,0,0,0}, {64,0,0,0}, {128,0,0,0},
            {27,0,0,0}, {54,0,0,0}, {108,0,0,0}
        };
        //recursivley encrypt each round and generate round key at end of iteration
        public void go(string plaintext, string key){

            //create 2d arrays
            byte[,] state = Make2DArray(plaintext.ToCharArray(),4,4);
            byte[,] key_arr = Make2DArray(key.ToCharArray(),4,4);
            string output = encrypt(state,key_arr);
            Console.WriteLine(output);

            //decryption x.x
            //state = Make2DArray(output.ToCharArray(),4,4);
            //output = encrypt(state,key_arr);
            
        }
        private string encrypt(byte[,] state, byte[,] key_array){
            //round 0
            state = XOrRoundKey(key_array,state);

            //rounds 1-10
            state = encrypt_round(state,key_array,0);

            string cyphertext = "";
            //maybe use a range for your indices throughout the project
            //so that you can iterate with a foreach
            //or create a key obj
            for(int col=0;col<4;col++){
                for(int row=0;row<4;row++){
                    cyphertext+=state[row,col]+" ";
                }
            }
            return cyphertext;
        }
        private byte[,] encrypt_round(byte[,] state, byte[,] round_key, int n){
            n++;
            state = SubBytes(state, "s-box.txt");
            state = ShiftRows(state);
            if(n!=10){
                state = MixColumns(state);
            }

            round_key = key_expansion(round_key,n-1);
            state = XOrRoundKey(state,round_key);
            
            //end case
            state  = n==10 ? state: encrypt_round(state,round_key,n);
            return state;
        }

        byte[,] ShiftRows(byte[,] state){
            byte[,] output = new byte[4,4];
            for(int row=0;row<4;row++){
                for(int b=0;b<4;b++){
                    output[row,(b-row+4)%4] = state[row,b];
                }
            }
            return output;
        }
        byte[,] SubBytes(byte[,] state, string infile){
            FileHelper f = new FileHelper();
            string raw_vals = f.FileInput(infile);
            string[] sbox = raw_vals.Split(" ");
            
            for(int i=0;i<4;i++){
                for(int j=0;j<4;j++){
                byte b = state[i,j];
                string bits = Convert.ToString(b,2).PadLeft(8,'0');
                int x = Convert.ToInt16(bits.Substring(0,4),2);
                int y = Convert.ToInt16(bits.Substring(4,4),2);
                int index = y+x*16;
                state[i,j] = (byte)Convert.ToInt16(sbox[index],16);
                }
            }
            return state;
        }
        string SubByte(byte b, string infile){
            FileHelper f = new FileHelper();
            string raw_vals = f.FileInput(infile);
            string[] sbox = raw_vals.Split(" ");

            string bits = Convert.ToString(b,2).PadLeft(8,'0');
            int x = Convert.ToInt16(bits.Substring(0,4),2);
            int y = Convert.ToInt16(bits.Substring(4,4),2);
            int index = y+x*16;
            string c = sbox[index];
            return c;
        }
        byte[,] MixColumns(byte[,] state){

            byte[,] k = new byte[4,4]{
                {2,3,1,1},
                {1,2,3,1},
                {1,1,2,3},
                {3,1,1,2},
            };

            byte[,] output = new byte[4,4];
            byte sum = 0;
            for(int column=0;column<4;column++){
                for(int i=0;i<4;i++){
                    sum = 0;
                    for(int b=0;b<4;b++){
                        sum = (byte)(sum^(gmul(state[b,column],k[i,b])));
                    }
                    output[i,column] = sum;
                }
            }
            return output;
        }
        //this function computes XOR addition between two blocks
        byte[,] XOrRoundKey(byte[,] a, byte[,] b){
            byte[,] output = new byte[4,4];
            for(int i=0;i<4;i++){
                for(int j=0;j<4;j++){
                    output[i,j] = (byte)(a[i,j] ^ b[i,j]);
                }
            }
            
            return output;
        }
        byte[,] key_expansion(byte[,] key, int n){
            byte[,] output = new byte[4,4];

            for(int word=0;word<4;word++){
                for(int b=0;b<4;b++){
                    if(word==0){
                        byte rot3 = key[(b+5)%4,word+3];
                        int s_byte = Convert.ToInt16(SubByte(rot3,"s-box.txt"),16);
                        int xorme = s_byte^rcon[n,b];
                        output[b,word] = (byte)(xorme^key[b,word]);
                    }
                    else{
                        output[b,word] = (byte)(key[b,word]^output[b,word-1]);
                    }
                }
            }
            return output;
        }
 
        //this fx computes bit multiplication
        int gmul(int a, int b){
            int product = 0;
            int high_bits;

            for(int i = 0;i<8;i++){
                //if the low bit of b is one
                if((b & 1) == 1){
                    product = product^a;
                }
                //0x80 reproductresents 128, stored to check if a is below 128
                high_bits = (a & 0x80);
                //shift bits one to the left
                a = a << 1;
                //0x1b is 27, 
                if(high_bits == 0x80){
                    a = a^0x1b;
                }
                b = b >> 1;
            }
            return product;
        }
        private  byte[,] Make2DArray(char[] input, int height, int width){
            byte[,] output = new byte[width, height];
            for (int w = 0; w < width; w++)
                for (int h = 0; h < height; h++)
                {
                    output[h, w] = (byte)input[w * height + h];
                }
            return output;
        }
        private  string Print2DArray(byte[,] input, int height, int width){
            string output = "";

            for (int w = 0; w < width; w++){
                output+="\n";
                for (int h = 0; h < height; h++)
                {
                    output+=Convert.ToString(input[w,h],16)+" ";
                }
            }
                return output;
        }
    }
}