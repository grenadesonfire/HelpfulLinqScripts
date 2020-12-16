<Query Kind="Program" />

void Main()
{
	var inputs = File.ReadAllLines(@"C:\Users\Nick\Documents\LINQPad Queries\GitLinq\AOC_2020\Inputs\Day4A");

	var convertList = PassPort.GetList(inputs);
	
	convertList.Count(l => l.SuperValid).Dump("Valid:");
}

public class PassPort{
	public int? BirthYear { get; set; }
	public int? IssueYear { get; set; }
	public int? ExpirationYear { get; set; }
	public string Height { get; set; }
	public string HairColor { get; set; }
	public string EyeColor { get; set; }
	public string PassportId { get; set; }
	public string CountryId { get; set; }

	public bool Valid
	{
		get
		{
			return
				BirthYear.HasValue
				&& IssueYear.HasValue
				&& ExpirationYear.HasValue
				&& !string.IsNullOrEmpty(Height)
				&& !string.IsNullOrEmpty(HairColor)
				&& !string.IsNullOrEmpty(EyeColor)
				&& !string.IsNullOrEmpty(PassportId);
				//&& !string.IsNullOrEmpty(CountryId);
		}
	}

	public bool SuperValid { 
		get{
			return 
				BirthYear.HasValue && BirthYear >= 1920 && BirthYear <= 2002
				&& IssueYear.HasValue && IssueYear >= 2010 && IssueYear <= 2020
				&& ExpirationYear.HasValue && ExpirationYear >= 2020 && ExpirationYear <= 2030
				&& !string.IsNullOrEmpty(Height) && ValidHeight(Height)
				&& !string.IsNullOrEmpty(HairColor) && ValidHair(HairColor)
				&& !string.IsNullOrEmpty(EyeColor) && ValidEye(EyeColor)
				&& !string.IsNullOrEmpty(PassportId) && ValidPid(PassportId);
				//&& !string.IsNullOrEmpty(CountryId);
		}
	}
	
	private bool ValidPid(string pid)
	{
		return pid.Length == 9 && !pid.Any(p => p < '0' || p > '9');
	}
	
	private bool ValidEye(string eye){
		return eye == "amb" 
			|| eye == "blu"
			|| eye == "brn"
			|| eye == "gry"
			|| eye == "grn"
			|| eye == "hzl"
			|| eye == "oth";
	}
	
	private bool ValidHair(string hair){
		return hair.Length == 7 
			&& hair[0] == '#' 
			&& !hair.Substring(1)
				.Any(h => 
					(h < '0' || h > '9') 
					&& (h < 'a' || h > 'f' ));
	}
	
	private bool ValidHeight(string height){
		var num = int.Parse(height.Substring(0, height.Length-2));
		
		if(height.Substring(height.Length-2) == "cm") return num >=150 && num <= 193;
		return num >= 59 && num <= 76;
	}
	
	public void SetQualities(string line){
		var qs = line.Split(' ');
		foreach(var q in qs){
			var qparts = q.Split(':');
			
			switch (qparts[0])
			{
				case "byr": BirthYear = int.Parse(qparts[1]); break;
				case "iyr": IssueYear = int.Parse(qparts[1]); break;
				case "eyr": ExpirationYear = int.Parse(qparts[1]); break;
				case "hgt": Height = qparts[1]; break;
				case "hcl": HairColor = qparts[1]; break;
				case "ecl": EyeColor = qparts[1]; break;
				case "pid": PassportId = qparts[1]; break;
				case "cid": CountryId = qparts[1]; break;
			}
		}
	}
	
	public static List<PassPort> GetList(string[] lines)
	{
		var ret = new List<PassPort>();
		var working = new PassPort();
		foreach(var line in lines){
			if(string.IsNullOrEmpty(line)){
				ret.Add(working);
				working = new PassPort();
			}
			else{
				working.SetQualities(line);
			}
		}
		return ret;
	}
}
