// requires Windows 7, Windows 7 Service Pack 1, Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Server 2008 R2, Windows Server 2008 R2 SP1, Windows Vista Service Pack 1, Windows XP Service Pack 3
// requires Windows Installer 3.1
// requires Internet Explorer 5.01
// WARNING: express setup (downloads and installs the components depending on your OS) if you want to deploy it on cd or network download the full bootsrapper on website below
// http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992

[CustomMessages]
xnafx40_title=XNA 4.0

xnafx40_size=7 MB

[Code]
const
	xnafx40_url = 'http://download.microsoft.com/download/A/C/2/AC2C903B-E6E8-42C2-9FD7-BEBAC362A930/xnafx40_redist.msi';

procedure xnafx40();
begin
	if (not IsXNAFrameworkDetected()) then
		AddProduct('xnafx40_redist.msi',
			'/qb',
			CustomMessage('xnafx40_title'),
			CustomMessage('xnafx40_size'),
			xnafx40_url,
			false, false);
end;