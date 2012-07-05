""" 
	DiplomacyDialog class handlers 
"""
class Window:
	"""
		init window
	"""
	def Initialize(self, parent):
		self.parent = parent

		self.InitControls()

	def InitControls(self):

		i = 0
		for player in game.Players:
			""" logo """
			leaderLogo = self.parent.GetControl("LeaderLogo" + str(i))
			if leaderLogo != None:
				leaderLogo.Image = player.Leader.Image

			""" title """
			title = self.parent.GetControl("LeaderName" + str(i))
			if title != None:
				if player.IsHuman:
					title.Text = "You"
				else:
					title.Text = player.Leader.Title

			""" civilization """
			civilization = self.parent.GetControl("CivilizationName" + str(i))
			if civilization != None:
				civilization.Text = player.Civilization.Title

			""" score """
			rank = self.parent.GetControl("CivRank" + str(i))
			if rank != None:
				rank.Text = player.Score.ToString()

			i = i + 1

""" create window instance """
window = Window()