using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio;
using Commons.Music.Midi;


namespace MediFilePlayer
{
    internal class MediPlayer : IMediPlayer
    {
        private static CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

        private static IMidiAccess access = MidiAccessManager.Default;
        private static IMidiOutput output = access.OpenOutputAsync(access.Outputs.Last().Id).Result;
        private MidiMusic music;
        private MidiPlayer player;
        private CancellationTokenSource cts;

        public MediPlayer(string fileName)
        {
            music = MidiMusic.Read(System.IO.File.OpenRead(fileName));
            player = new MidiPlayer(music, output);
        }

        public void startPlaying()
        {
            MidiEventAction displayTracks = (MidiEvent e) =>
            {
                if (e.EventType == MidiEvent.Program)
                    Console.WriteLine($"Track/Channel: {e.Channel}: Instrment Code: {e.Msb}, Instrument Name: {InstrumentsCode[e.Msb]}");
            };
            Console.WriteLine($"Song Length: {music.GetTotalPlayTimeMilliseconds() / 1000}s");
            player.EventReceived += displayTracks;
            cts = new CancellationTokenSource();

            ThreadPool.QueueUserWorkItem(new WaitCallback(loop), cts.Token);
            Thread.Sleep(100);
            player.EventReceived -= displayTracks;
        }

        public void play()
        {
            cts = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(new WaitCallback(loop), cts.Token);
        }

        public void pause()
        {
            cts.Cancel();
            cts.Dispose();
        }

        public void stopPlaying()
        {
            cts.Cancel();
            cts.Dispose();
            player.Dispose();
        }

        public void increaseVolume()
        {
            defaultPlaybackDevice.Volume += 5;
            Console.WriteLine("Volume:" + defaultPlaybackDevice.Volume);
        }

        public void decreaseVolume()
        {
            defaultPlaybackDevice.Volume -= 5;
            Console.WriteLine("Volume:" + defaultPlaybackDevice.Volume);
        }

        public void changeMusic(string fileName)
        {
            music = MidiMusic.Read(System.IO.File.OpenRead(fileName));
            player = new MidiPlayer(music, output);
            startPlaying();
        }

        public void loop(object? obj)
        {
            if (obj is null)
            {
                return;
            }
            player.Play();
            bool looping = true;
            CancellationToken token = (CancellationToken)obj;
            token.Register(() =>
            {
                looping = false;
                player.Pause();
            });
            while (looping == true)
            {
                while (looping)
                {
                    Thread.Sleep(100);
                    if (music.GetTotalPlayTimeMilliseconds() <= (int)(player.PositionInTime.TotalSeconds * 1000))
                    {
                        break;
                    }
                }
                if (looping == true)
                {
                    player.Dispose();
                    player.Play();
                }
            }
        }

        private static readonly Dictionary<long, string> InstrumentsCode = new Dictionary<long, string>
        {
            { 0, "Acoustic Grand Piano"},
            { 1, "Bright Acoustic Piano"},
            { 2, "Electric Grand Piano"},
            { 3, "Honky-tonk Piano"},
            { 4, "Rhodes Piano"},
            { 5, "Chorused Piano"},
            { 6, "Harpsichord"},
            { 7, "Clavinet"},
            { 8, "Celesta"},
            { 9, "Glockenspiel"},
            { 10, "Music box"},
            { 11, "Vibraphone"},
            { 12, "Marimba"},
            { 13, "Xylophone"},
            { 14, "Tubular Bells"},
            { 15, "Dulcimer"},
            { 16, "Hammond Organ"},
            { 17, "Percussive Organ"},
            { 18, "Rock Organ"},
            { 19, "Church Organ"},
            { 20, "Reed Organ"},
            { 21, "Accordion"},
            { 22, "Harmonica"},
            { 23, "Tango Accordion"},
            { 24, "Acoustic Guitar (nylon)"},
            { 25, "Acoustic Guitar (steel)"},
            { 26, "Electric Guitar (jazz)"},
            { 27, "Electric Guitar (clean)"},
            { 28, "Electric Guitar (muted)"},
            { 29, "Overdriven Guitar"},
            { 30, "Distortion Guitar"},
            { 31, "Guitar Harmonics"},
            { 32, "Acoustic Bass"},
            { 33, "Electric Bass (finger)"},
            { 34, "Electric Bass (pick)"},
            { 35, "Fretless Bass"},
            { 36, "Slap Bass 1"},
            { 37, "Slap Bass 2"},
            { 38, "Synth Bass 1"},
            { 39, "Synth Bass 2"},
            { 40, "Violin"},
            { 41, "Viola"},
            { 42, "Cello"},
            { 43, "Contrabass"},
            { 44, "Tremolo Strings"},
            { 45, "Pizzicato Strings"},
            { 46, "Orchestral Harp"},
            { 47, "Timpani"},
            { 48, "String Ensemble 1"},
            { 49, "String Ensemble 2"},
            { 50, "Synth Strings 1"},
            { 51, "Synth Strings 2"},
            { 52, "Choir Aahs"},
            { 53, "Voice Oohs"},
            { 54, "Synth Voice"},
            { 55, "Orchestra Hit"},
            { 56, "Trumpet"},
            { 57, "Trombone"},
            { 58, "Tuba"},
            { 59, "Muted Trumpet"},
            { 60, "French Horn"},
            { 61, "Brass Section"},
            { 62, "Synth Brass 1"},
            { 63, "Synth Brass 2"},
            { 64, "Soprano Sax"},
            { 65, "Alto Sax"},
            { 66, "Tenor Sax"},
            { 67, "Baritone Sax"},
            { 68, "Oboe"},
            { 69, "English Horn"},
            { 70, "Bassoon"},
            { 71, "Clarinet"},
            { 72, "Piccolo"},
            { 73, "Flute"},
            { 74, "Recorder"},
            { 75, "Pan Flute"},
            { 76, "Bottle Blow"},
            { 77, "Shakuhachi"},
            { 78, "Whistle" },
            { 79, "Ocarina"},
            { 80, "Lead 1 (square)"},
            { 81, " Lead 2 (sawtooth)"},
            { 82, "Lead 3 (calliope lead)"},
            { 83, "Lead 4 (chiffer lead)"},
            { 84, "Lead 5 (charang)"},
            { 85, "Lead 6 (voice)"},
            { 86, "Lead 7 (fifths)"},
            { 87, "Lead 8 (brass + lead)"},
            { 88, "Pad 1 (new age)"},
            { 89, "Pad 2 (warm)"},
            { 90, "Pad 3 (polysynth)"},
            { 91, "Pad 4 (choir)"},
            { 92, "Pad 5 (bowed)"},
            { 93, "Pad 6 (metallic)"},
            { 94, "Pad 7 (halo)"},
            { 95, "Pad 8 (sweep)"},
            { 96, "FX 1 (rain)"},
            { 97, "FX 2 (soundtrack)"},
            { 98, "FX 3 (crystal)"},
            { 99, "FX 4 (atmosphere)"},
            { 100, "FX 5 (brightness)"},
            { 101, "FX 6 (goblins)"},
            { 102, "FX 7 (echoes)"},
            { 103, "FX 8 (sci-fi)"},
            { 104, "Sitar"},
            { 105, "Banjo"},
            { 106, "Shamisen"},
            { 107, "Koto"},
            { 108, "Kalimba"},
            { 109, "Bagpipe"},
            { 110, "Fiddle"},
            { 111, "Shana"},
            { 112, "Tinkle Bell"},
            { 113, "Agogo"},
            { 114, "Steel Drums"},
            { 115, "Woodblock"},
            { 116, "Taiko Drum"},
            { 117, "Melodic Tom"},
            { 118, "Synth Drum"},
            { 119, "Reverse Cymbal"},
            { 120, "Guitar Fret Noise"},
            { 121, "Breath Noise"},
            { 122, "Seashore"},
            { 123, "Bird Tweet"},
            { 124, "Telephone Ring"},
            { 125, "Helicopter"},
            { 126, "Applause1"},
            { 127, "Gunshot"},
        };
    }
}
