﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AirBreather.Text
{
    public static class Base32768
    {
        // each char in this string is the UTF-16 code unit that marks the start of a range of 32
        // code points in the BMP which are appropriate for encoding.  there are a total of 1,024
        // such ranges, for a total of 32,768 available chars.
        private const string RangeStarts_NonAscii = "ҠԀڀڠݠހ߀ကႠᄀᄠᅀᆀᇠሀሠበዠጠᎠᏀᐠᑀᑠᒀᒠᓀᓠᔀᔠᕀᕠᖀᖠᗀᗠᘀᘠᙀᚠᛀកᠠᡀᣀᦀ᧠ᨠᯀᰀᴀ⇠⋀⍀⍠⎀⎠⏀␀─┠╀╠▀■◀◠☀☠♀♠⚀⚠⛀⛠✀✠❀➀➠⠀⠠⡀⡠⢀⢠⣀⣠⤀⤠⥀⥠⦠⨠⩀⪀⪠⫠⬀⬠⭀ⰀⲀⲠⳀⴀⵀ⺠⻀㇀㐀㐠㑀㑠㒀㒠㓀㓠㔀㔠㕀㕠㖀㖠㗀㗠㘀㘠㙀㙠㚀㚠㛀㛠㜀㜠㝀㝠㞀㞠㟀㟠㠀㠠㡀㡠㢀㢠㣀㣠㤀㤠㥀㥠㦀㦠㧀㧠㨀㨠㩀㩠㪀㪠㫀㫠㬀㬠㭀㭠㮀㮠㯀㯠㰀㰠㱀㱠㲀㲠㳀㳠㴀㴠㵀㵠㶀㶠㷀㷠㸀㸠㹀㹠㺀㺠㻀㻠㼀㼠㽀㽠㾀㾠㿀㿠䀀䀠䁀䁠䂀䂠䃀䃠䄀䄠䅀䅠䆀䆠䇀䇠䈀䈠䉀䉠䊀䊠䋀䋠䌀䌠䍀䍠䎀䎠䏀䏠䐀䐠䑀䑠䒀䒠䓀䓠䔀䔠䕀䕠䖀䖠䗀䗠䘀䘠䙀䙠䚀䚠䛀䛠䜀䜠䝀䝠䞀䞠䟀䟠䠀䠠䡀䡠䢀䢠䣀䣠䤀䤠䥀䥠䦀䦠䧀䧠䨀䨠䩀䩠䪀䪠䫀䫠䬀䬠䭀䭠䮀䮠䯀䯠䰀䰠䱀䱠䲀䲠䳀䳠䴀䴠䵀䵠䶀䷀䷠一丠乀习亀亠什仠伀传佀你侀侠俀俠倀倠偀偠傀傠僀僠儀儠兀兠冀冠净几刀删剀剠劀加勀勠匀匠區占厀厠叀叠吀吠呀呠咀咠哀哠唀唠啀啠喀喠嗀嗠嘀嘠噀噠嚀嚠囀因圀圠址坠垀垠埀埠堀堠塀塠墀墠壀壠夀夠奀奠妀妠姀姠娀娠婀婠媀媠嫀嫠嬀嬠孀孠宀宠寀寠尀尠局屠岀岠峀峠崀崠嵀嵠嶀嶠巀巠帀帠幀幠庀庠廀廠开张彀彠往徠忀忠怀怠恀恠悀悠惀惠愀愠慀慠憀憠懀懠戀戠所扠技抠拀拠挀挠捀捠掀掠揀揠搀搠摀摠撀撠擀擠攀攠敀敠斀斠旀无昀映晀晠暀暠曀曠最朠杀杠枀枠柀柠栀栠桀桠梀梠检棠椀椠楀楠榀榠槀槠樀樠橀橠檀檠櫀櫠欀欠歀歠殀殠毀毠氀氠汀池沀沠泀泠洀洠浀浠涀涠淀淠渀渠湀湠満溠滀滠漀漠潀潠澀澠激濠瀀瀠灀灠炀炠烀烠焀焠煀煠熀熠燀燠爀爠牀牠犀犠狀狠猀猠獀獠玀玠珀珠琀琠瑀瑠璀璠瓀瓠甀甠畀畠疀疠痀痠瘀瘠癀癠皀皠盀盠眀眠着睠瞀瞠矀矠砀砠础硠碀碠磀磠礀礠祀祠禀禠秀秠稀稠穀穠窀窠竀章笀笠筀筠简箠節篠簀簠籀籠粀粠糀糠紀素絀絠綀綠緀締縀縠繀繠纀纠绀绠缀缠罀罠羀羠翀翠耀耠聀聠肀肠胀胠脀脠腀腠膀膠臀臠舀舠艀艠芀芠苀苠茀茠荀荠莀莠菀菠萀萠葀葠蒀蒠蓀蓠蔀蔠蕀蕠薀薠藀藠蘀蘠虀虠蚀蚠蛀蛠蜀蜠蝀蝠螀螠蟀蟠蠀蠠血衠袀袠裀裠褀褠襀襠覀覠觀觠言訠詀詠誀誠諀諠謀謠譀譠讀讠诀诠谀谠豀豠貀負賀賠贀贠赀赠趀趠跀跠踀踠蹀蹠躀躠軀軠輀輠轀轠辀辠迀迠退造遀遠邀邠郀郠鄀鄠酀酠醀醠釀釠鈀鈠鉀鉠銀銠鋀鋠錀錠鍀鍠鎀鎠鏀鏠鐀鐠鑀鑠钀钠铀铠销锠镀镠門閠闀闠阀阠陀陠隀隠雀雠需霠靀靠鞀鞠韀韠頀頠顀顠颀颠飀飠餀餠饀饠馀馠駀駠騀騠驀驠骀骠髀髠鬀鬠魀魠鮀鮠鯀鯠鰀鰠鱀鱠鲀鲠鳀鳠鴀鴠鵀鵠鶀鶠鷀鷠鸀鸠鹀鹠麀麠黀黠鼀鼠齀齠龀龠ꀀꀠꁀꁠꂀꂠꃀꃠꄀꄠꅀꅠꆀꆠꇀꇠꈀꈠꉀꉠꊀꊠꋀꋠꌀꌠꍀꍠꎀꎠꏀꏠꐀꐠꑀꑠ꒠ꔀꔠꕀꕠꖀꖠꗀꗠꙀꚠꛀ꜀꜠ꝀꞀꡀ";

        // a counterpart to the previous one, which lets us optimize for ASCII chars.
        private const string RangeStarts_Ascii = "ƀɀɠʀ";

        private static readonly Lazy<(Maps nonAscii, Maps ascii)> ComputedMaps = new Lazy<(Maps nonAscii, Maps ascii)>(() => (nonAscii: Maps.Build(RangeStarts_NonAscii), ascii: Maps.Build(RangeStarts_Ascii)));

        public static string Encode(ReadOnlySpan<byte> data)
        {
            (Maps nonAscii, Maps ascii) = ComputedMaps.Value;
            var sb = new StringBuilder();
            int bitOffset = 0;
            while (data.Length > 2 || (data.Length == 2 && bitOffset < 2))
            {
                // TODO: reverse bits in byte?
                int indexBase = data[0] | (data[1] << 8);
                if (bitOffset > 1)
                {
                    indexBase |= data[2] << 16;
                    data = data.Slice(3);
                }
                else
                {
                    data = data.Slice(2);
                }

                short index = unchecked((short)(indexBase >> bitOffset));
                sb.Append(index < 128 ? ascii.Encode[index] : nonAscii.Encode[index]);
                if (--bitOffset == -1)
                {
                    bitOffset = 7;
                }
            }

            if (data.Length > 0)
            {
                // TODO: reverse bits in byte?
                short index = data[0];
                if (data.Length == 2)
                {
                    index = unchecked((short)(data[1] << 8));
                }

                sb.Append(index < 128 ? ascii.Encode[index] : nonAscii.Encode[index]);
            }

            return sb.ToString();
        }

        private readonly struct Maps
        {
            public readonly Dictionary<short, char> Encode;

            public readonly Dictionary<char, short> Decode;

            private Maps(Dictionary<short, char> encode, Dictionary<char, short> decode)
            {
                this.Encode = encode;
                this.Decode = decode;
            }

            public static Maps Build(string rangeStarts)
            {
                var encode = new Dictionary<short, char>(rangeStarts.Length * 32);
                var decode = new Dictionary<char, short>(rangeStarts.Length * 32);
                for (int i = 0; i < rangeStarts.Length; i++)
                {
                    char rangeStart = rangeStarts[i];
                    int blockStart = i * 32;
                    for (int j = 0; j < 32; j++)
                    {
                        char ch = unchecked((char)(rangeStart + j));
                        short enc = unchecked((short)(blockStart + j));
                        encode[enc] = ch;
                        decode[ch] = enc;
                    }
                }

                return new Maps(encode, decode);
            }
        }
    }
}
