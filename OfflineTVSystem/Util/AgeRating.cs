namespace OTS.Util
{
    //https://en.wikipedia.org/wiki/Motion_picture_content_rating_system
    /// <summary>Age Ratings for each country, for both movies and tv shows</summary>
    public enum AgeRating
    {
        /// <summary>No age rating or non provided, default value</summary>
        NOT_RATED = 0,
        #region TV Parental Guidelines (US)
        //http://www.tvguidelines.org/
        /// <summary>Intended for ages 2 to 6.</summary>
        TV_Y = 1,
        /// <summary>Intended for ages 7 and older.</summary>
        TV_Y7 = 2,
        /// <summary>Intended for ages 7 and older. Contains fantasy violence more combative than TV_Y7 programs</summary>
        TV_Y7FV = 3,
        /// <summary>Intended for all ages</summary>
        TV_G = 4,
        /// <summary>Intended for younger children in the company of an adult.</summary>
        TV_PG = 5,
        ///<summary>Intended for children ages 14 and older.</summary>
        TV_14 = 6,
        /// <summary>Intended for adults.</summary>
        TV_MA = 7,
        #endregion
        #region Motion Pictures Association of America
        //https://www.motionpictures.org/film-ratings/
        /// <summary>General audiences, all ages</summary>
        MPAA_G = 8,
        /// <summary>Parental guidance suggested, some material may not be suitable for children</summary>
        MPAA_PG = 9,
        /// <summary>ages 13 and over, parent guidance suggested</summary>
        MPAA_PG13 = 10,
        /// <summary>Restricted, under 17 requires accompanying parent or adult gaurdian</summary>
        MPAA_R = 11,
        /// <summary>Adults only, no one under 17</summary>
        MPAA_NC17 = 12,
        #endregion
        #region BBFC (UK)
        //https://www.bbfc.co.uk/about-classification/classification-guidelines
        /// <summary>Universal, suitable for all ages.</summary>
        BB_U = 13,
        /// <summary>Parental Guidance suggested.</summary>
        BB_PG = 14,
        /// <summary>Video ages 12 and over.</summary>
        BB_12 = 15,
        /// <summary>Cinema ages 12 and over.</summary>
        BB_12A = 16,
        /// <summary>Ages 15 and over.</summary>
        BB_15 = 17,
        /// <summary>Ages 18 and over, adults only.</summary>
        BB_18 = 18,
        /// <summary>Adults only, for licensed premises only</summary>
        BB_R18 = 19,
        #endregion
        #region Kijkwijzer (NL)
        /// <summary>All ages</summary>
        KIJK_AL = 20,
        /// <summary>Ages 6 and older.</summary>
        KIJK_6 = 21,
        /// <summary>Ages 9 and older</summary>
        KIJK_9 = 22,
        /// <summary>Ages 12 and older.</summary>
        KIJK_12 = 23,
        /// <summary>Ages 14 and older.</summary>
        KIJK_14 = 24,
        /// <summary>Ages 16 and older.</summary>
        KIJK_16 = 25,
        /// <summary>Ages 18 and older.</summary>
        KIJK_18 = 26,
        #endregion
        #region Freiwillige Selbstkontrolle Fernsehen - FSF(DE)
        //https://en.fsf.de/media-classification/fsf-age-ratings/
        #endregion
        #region CERO (JP)
        #endregion
    }
}
